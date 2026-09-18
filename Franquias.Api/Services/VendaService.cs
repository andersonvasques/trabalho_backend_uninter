using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras de venda.
///
/// Regras aplicadas:
/// - a venda pertence a uma única unidade e precisa de pelo menos um item;
/// - unidades inativas não podem registrar vendas;
/// - o valor total é calculado a partir dos itens, quantidades e preços;
/// - o estoque é baixado na confirmação e devolvido no cancelamento;
/// - o estoque nunca pode ficar negativo.
/// </summary>
public sealed class VendaService : IVendaService
{
    private readonly IVendaRepositorio _vendaRepositorio;
    private readonly IUnidadeRepositorio _unidadeRepositorio;
    private readonly IProdutoRepositorio _produtoRepositorio;
    private readonly IEstoqueRepositorio _estoqueRepositorio;
    private readonly IUsuarioContexto _usuarioContexto;

    public VendaService(
        IVendaRepositorio vendaRepositorio,
        IUnidadeRepositorio unidadeRepositorio,
        IProdutoRepositorio produtoRepositorio,
        IEstoqueRepositorio estoqueRepositorio,
        IUsuarioContexto usuarioContexto)
    {
        _vendaRepositorio = vendaRepositorio;
        _unidadeRepositorio = unidadeRepositorio;
        _produtoRepositorio = produtoRepositorio;
        _estoqueRepositorio = estoqueRepositorio;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<ResultadoPaginado<VendaDto>> ListarAsync(
        ConsultaVendasDto filtros,
        CancellationToken cancellationToken)
    {
        filtros.UnidadeFranqueadaId = _usuarioContexto.ResolverFiltroDeUnidade(filtros.UnidadeFranqueadaId);

        ResultadoPaginado<Venda> pagina =
            await _vendaRepositorio.ListarPaginadoAsync(filtros, cancellationToken);

        var itens = new List<VendaDto>();

        foreach (Venda venda in pagina.Itens)
        {
            itens.Add(VendaDto.DeModelo(venda));
        }

        return new ResultadoPaginado<VendaDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<VendaDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        Venda venda = await ObterVendaAsync(id, cancellationToken);

        return VendaDto.DeModelo(venda);
    }

    public async Task<VendaDto> RegistrarAsync(
        CriarVendaDto dados,
        CancellationToken cancellationToken)
    {
        _usuarioContexto.GarantirAcessoAUnidade(dados.UnidadeFranqueadaId);

        if (dados.Itens.Count == 0)
        {
            throw new RegraDeNegocioException("A venda deve possuir pelo menos um item.");
        }

        UnidadeFranqueada? unidade = await _unidadeRepositorio.ObterPorIdAsync(
            dados.UnidadeFranqueadaId,
            cancellationToken);

        if (unidade is null)
        {
            throw new NaoEncontradoException($"A unidade {dados.UnidadeFranqueadaId} não foi encontrada.");
        }

        // Regra de negócio: uma unidade inativa não pode registrar novas vendas.
        if (!unidade.PodeRegistrarVenda())
        {
            throw new RegraDeNegocioException(
                $"A unidade {unidade.Codigo} está com a situação {unidade.Situacao} " +
                "e não pode registrar novas vendas.");
        }

        var venda = new Venda
        {
            Numero = await _vendaRepositorio.GerarNumeroAsync(cancellationToken),
            UnidadeFranqueadaId = unidade.Id,
            UsuarioId = _usuarioContexto.UsuarioId,
            DataVenda = dados.DataVenda ?? DateTime.UtcNow,
            Status = StatusVenda.Confirmada,
            FormaPagamento = dados.FormaPagamento,
            Desconto = decimal.Round(dados.Desconto, 2),
            Cliente = dados.Cliente?.Trim(),
            Observacao = dados.Observacao?.Trim(),
            CriadaEm = DateTime.UtcNow
        };

        foreach (CriarItemVendaDto itemInformado in dados.Itens)
        {
            ProdutoServico? produto = await _produtoRepositorio.ObterPorIdAsync(
                itemInformado.ProdutoServicoId,
                cancellationToken);

            if (produto is null)
            {
                throw new NaoEncontradoException(
                    $"O produto {itemInformado.ProdutoServicoId} não foi encontrado.");
            }

            if (!produto.Ativo)
            {
                throw new RegraDeNegocioException(
                    $"O item '{produto.Nome}' está inativo e não pode ser vendido.");
            }

            decimal precoUnitario = itemInformado.PrecoUnitario ?? produto.PrecoBase;

            var item = new ItemVenda
            {
                ProdutoServicoId = produto.Id,
                DescricaoItem = produto.Nome,
                Quantidade = itemInformado.Quantidade,
                PrecoUnitario = decimal.Round(precoUnitario, 2)
            };

            item.CalcularSubtotal();

            venda.Itens.Add(item);
        }

        // O valor total sempre vem dos itens; o cliente não escolhe o total.
        venda.RecalcularTotal();

        await _vendaRepositorio.AdicionarAsync(venda, cancellationToken);

        await BaixarEstoqueAsync(venda, cancellationToken);

        await _vendaRepositorio.SalvarAlteracoesAsync(cancellationToken);

        Venda? registrada = await _vendaRepositorio.ObterCompletaAsync(venda.Id, cancellationToken);

        return VendaDto.DeModelo(registrada ?? venda);
    }

    public async Task<VendaDto> CancelarAsync(
        int id,
        CancelarVendaDto dados,
        CancellationToken cancellationToken)
    {
        Venda venda = await ObterVendaAsync(id, cancellationToken);

        venda.Cancelar(dados.Motivo.Trim());

        await DevolverEstoqueAsync(venda, cancellationToken);

        await _vendaRepositorio.SalvarAlteracoesAsync(cancellationToken);

        Venda? cancelada = await _vendaRepositorio.ObterCompletaAsync(id, cancellationToken);

        return VendaDto.DeModelo(cancelada ?? venda);
    }

    /// <summary>
    /// Dá baixa no estoque de cada item vendido e registra as movimentações.
    /// </summary>
    private async Task BaixarEstoqueAsync(Venda venda, CancellationToken cancellationToken)
    {
        foreach (ItemVenda item in venda.Itens)
        {
            ProdutoServico? produto = await _produtoRepositorio.ObterPorIdAsync(
                item.ProdutoServicoId,
                cancellationToken);

            if (produto is null || !produto.ControlaEstoque())
            {
                // Serviços não movimentam estoque.
                continue;
            }

            EstoqueUnidade? estoque = await _estoqueRepositorio.ObterParaAtualizacaoAsync(
                venda.UnidadeFranqueadaId,
                item.ProdutoServicoId,
                cancellationToken);

            if (estoque is null)
            {
                throw new RegraDeNegocioException(
                    $"O produto '{produto.Nome}' não possui saldo de estoque cadastrado nesta unidade.");
            }

            int saldoAnterior = estoque.Quantidade;

            estoque.Debitar(item.Quantidade);

            var movimentacao = new MovimentacaoEstoque
            {
                EstoqueUnidade = estoque,
                Tipo = TipoMovimentacaoEstoque.Saida,
                Quantidade = item.Quantidade,
                SaldoAnterior = saldoAnterior,
                SaldoAtual = estoque.Quantidade,
                Motivo = $"Baixa automática da venda {venda.Numero}.",
                Venda = venda,
                UsuarioId = venda.UsuarioId,
                OcorridaEm = DateTime.UtcNow
            };

            await _estoqueRepositorio.AdicionarMovimentacaoAsync(movimentacao, cancellationToken);
        }
    }

    /// <summary>
    /// Devolve ao estoque os itens de uma venda cancelada.
    /// </summary>
    private async Task DevolverEstoqueAsync(Venda venda, CancellationToken cancellationToken)
    {
        foreach (ItemVenda item in venda.Itens)
        {
            ProdutoServico? produto = await _produtoRepositorio.ObterPorIdAsync(
                item.ProdutoServicoId,
                cancellationToken);

            if (produto is null || !produto.ControlaEstoque())
            {
                continue;
            }

            EstoqueUnidade? estoque = await _estoqueRepositorio.ObterParaAtualizacaoAsync(
                venda.UnidadeFranqueadaId,
                item.ProdutoServicoId,
                cancellationToken);

            if (estoque is null)
            {
                continue;
            }

            int saldoAnterior = estoque.Quantidade;

            estoque.Creditar(item.Quantidade);

            var movimentacao = new MovimentacaoEstoque
            {
                EstoqueUnidade = estoque,
                Tipo = TipoMovimentacaoEstoque.Entrada,
                Quantidade = item.Quantidade,
                SaldoAnterior = saldoAnterior,
                SaldoAtual = estoque.Quantidade,
                Motivo = $"Devolução pelo cancelamento da venda {venda.Numero}.",
                VendaId = venda.Id,
                UsuarioId = _usuarioContexto.UsuarioId,
                OcorridaEm = DateTime.UtcNow
            };

            await _estoqueRepositorio.AdicionarMovimentacaoAsync(movimentacao, cancellationToken);
        }
    }

    private async Task<Venda> ObterVendaAsync(int id, CancellationToken cancellationToken)
    {
        Venda? venda = await _vendaRepositorio.ObterCompletaAsync(id, cancellationToken);

        if (venda is null)
        {
            throw new NaoEncontradoException($"A venda {id} não foi encontrada.");
        }

        _usuarioContexto.GarantirAcessoAUnidade(venda.UnidadeFranqueadaId);

        return venda;
    }
}
