using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras de controle de estoque.
///
/// Toda alteração de saldo gera um registro no histórico de movimentações,
/// permitindo auditar entradas, saídas e ajustes.
/// </summary>
public sealed class EstoqueService : IEstoqueService
{
    private readonly IEstoqueRepositorio _estoqueRepositorio;
    private readonly IProdutoRepositorio _produtoRepositorio;
    private readonly IUnidadeRepositorio _unidadeRepositorio;
    private readonly IUsuarioContexto _usuarioContexto;

    public EstoqueService(
        IEstoqueRepositorio estoqueRepositorio,
        IProdutoRepositorio produtoRepositorio,
        IUnidadeRepositorio unidadeRepositorio,
        IUsuarioContexto usuarioContexto)
    {
        _estoqueRepositorio = estoqueRepositorio;
        _produtoRepositorio = produtoRepositorio;
        _unidadeRepositorio = unidadeRepositorio;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<ResultadoPaginado<EstoqueDto>> ListarAsync(
        ConsultaEstoqueDto filtros,
        CancellationToken cancellationToken)
    {
        filtros.UnidadeFranqueadaId = _usuarioContexto.ResolverFiltroDeUnidade(filtros.UnidadeFranqueadaId);

        ResultadoPaginado<EstoqueUnidade> pagina =
            await _estoqueRepositorio.ListarPaginadoAsync(filtros, cancellationToken);

        var itens = new List<EstoqueDto>();

        foreach (EstoqueUnidade estoque in pagina.Itens)
        {
            itens.Add(EstoqueDto.DeModelo(estoque));
        }

        return new ResultadoPaginado<EstoqueDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<EstoqueDto> ObterSaldoAsync(
        int unidadeId,
        int produtoId,
        CancellationToken cancellationToken)
    {
        _usuarioContexto.GarantirAcessoAUnidade(unidadeId);

        EstoqueUnidade? estoque = await _estoqueRepositorio.ObterParaAtualizacaoAsync(
            unidadeId,
            produtoId,
            cancellationToken);

        if (estoque is null)
        {
            throw new NaoEncontradoException(
                $"O produto {produtoId} não possui saldo cadastrado na unidade {unidadeId}.");
        }

        return EstoqueDto.DeModelo(estoque);
    }

    public async Task<EstoqueDto> MovimentarAsync(
        MovimentarEstoqueDto dados,
        CancellationToken cancellationToken)
    {
        _usuarioContexto.GarantirAcessoAUnidade(dados.UnidadeFranqueadaId);

        UnidadeFranqueada unidade = await ObterUnidadeAsync(dados.UnidadeFranqueadaId, cancellationToken);
        ProdutoServico produto = await ObterProdutoAsync(dados.ProdutoServicoId, cancellationToken);

        if (!produto.ControlaEstoque())
        {
            throw new RegraDeNegocioException(
                $"O item '{produto.Nome}' é um serviço e não controla estoque.");
        }

        EstoqueUnidade? estoque = await _estoqueRepositorio.ObterParaAtualizacaoAsync(
            unidade.Id,
            produto.Id,
            cancellationToken);

        if (estoque is null)
        {
            if (dados.Tipo == TipoMovimentacaoEstoque.Saida)
            {
                throw new RegraDeNegocioException(
                    $"O produto '{produto.Nome}' ainda não possui saldo na unidade {unidade.Codigo}.");
            }

            estoque = new EstoqueUnidade
            {
                UnidadeFranqueadaId = unidade.Id,
                ProdutoServicoId = produto.Id,
                ProdutoServico = produto,
                Quantidade = 0,
                QuantidadeMinima = produto.EstoqueMinimoPadrao,
                AtualizadoEm = DateTime.UtcNow
            };

            await _estoqueRepositorio.AdicionarAsync(estoque, cancellationToken);
        }

        int saldoAnterior = estoque.Quantidade;

        switch (dados.Tipo)
        {
            case TipoMovimentacaoEstoque.Entrada:
                estoque.Creditar(dados.Quantidade);
                break;

            case TipoMovimentacaoEstoque.Saida:
                // Regra de negócio: o estoque não pode ficar negativo.
                estoque.Debitar(dados.Quantidade);
                break;

            case TipoMovimentacaoEstoque.Ajuste:
                estoque.Ajustar(dados.Quantidade);
                break;

            default:
                throw new RegraDeNegocioException("Tipo de movimentação inválido.");
        }

        var movimentacao = new MovimentacaoEstoque
        {
            EstoqueUnidade = estoque,
            Tipo = dados.Tipo,
            Quantidade = dados.Quantidade,
            SaldoAnterior = saldoAnterior,
            SaldoAtual = estoque.Quantidade,
            Motivo = string.IsNullOrWhiteSpace(dados.Motivo)
                ? $"Movimentação manual ({dados.Tipo})."
                : dados.Motivo.Trim(),
            UsuarioId = _usuarioContexto.UsuarioId,
            OcorridaEm = DateTime.UtcNow
        };

        await _estoqueRepositorio.AdicionarMovimentacaoAsync(movimentacao, cancellationToken);
        await _estoqueRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return EstoqueDto.DeModelo(estoque);
    }

    public async Task<EstoqueDto> DefinirEstoqueMinimoAsync(
        DefinirEstoqueMinimoDto dados,
        CancellationToken cancellationToken)
    {
        _usuarioContexto.GarantirAcessoAUnidade(dados.UnidadeFranqueadaId);

        UnidadeFranqueada unidade = await ObterUnidadeAsync(dados.UnidadeFranqueadaId, cancellationToken);
        ProdutoServico produto = await ObterProdutoAsync(dados.ProdutoServicoId, cancellationToken);

        EstoqueUnidade? estoque = await _estoqueRepositorio.ObterParaAtualizacaoAsync(
            unidade.Id,
            produto.Id,
            cancellationToken);

        if (estoque is null)
        {
            estoque = new EstoqueUnidade
            {
                UnidadeFranqueadaId = unidade.Id,
                ProdutoServicoId = produto.Id,
                ProdutoServico = produto,
                Quantidade = 0,
                QuantidadeMinima = dados.QuantidadeMinima,
                AtualizadoEm = DateTime.UtcNow
            };

            await _estoqueRepositorio.AdicionarAsync(estoque, cancellationToken);
        }
        else
        {
            estoque.QuantidadeMinima = dados.QuantidadeMinima;
            estoque.AtualizadoEm = DateTime.UtcNow;
        }

        await _estoqueRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return EstoqueDto.DeModelo(estoque);
    }

    public async Task<IReadOnlyCollection<EstoqueDto>> ListarAbaixoDoMinimoAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        int? filtro = _usuarioContexto.ResolverFiltroDeUnidade(unidadeId);

        IReadOnlyCollection<EstoqueUnidade> estoques =
            await _estoqueRepositorio.ListarAbaixoDoMinimoAsync(filtro, cancellationToken);

        var itens = new List<EstoqueDto>();

        foreach (EstoqueUnidade estoque in estoques)
        {
            itens.Add(EstoqueDto.DeModelo(estoque));
        }

        return itens;
    }

    public async Task<ResultadoPaginado<MovimentacaoEstoqueDto>> ListarMovimentacoesAsync(
        ConsultaMovimentacoesDto filtros,
        CancellationToken cancellationToken)
    {
        filtros.UnidadeFranqueadaId = _usuarioContexto.ResolverFiltroDeUnidade(filtros.UnidadeFranqueadaId);

        ResultadoPaginado<MovimentacaoEstoque> pagina =
            await _estoqueRepositorio.ListarMovimentacoesAsync(filtros, cancellationToken);

        var itens = new List<MovimentacaoEstoqueDto>();

        foreach (MovimentacaoEstoque movimentacao in pagina.Itens)
        {
            itens.Add(MovimentacaoEstoqueDto.DeModelo(movimentacao));
        }

        return new ResultadoPaginado<MovimentacaoEstoqueDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    private async Task<UnidadeFranqueada> ObterUnidadeAsync(int id, CancellationToken cancellationToken)
    {
        UnidadeFranqueada? unidade = await _unidadeRepositorio.ObterPorIdAsync(id, cancellationToken);

        if (unidade is null)
        {
            throw new NaoEncontradoException($"A unidade {id} não foi encontrada.");
        }

        return unidade;
    }

    private async Task<ProdutoServico> ObterProdutoAsync(int id, CancellationToken cancellationToken)
    {
        ProdutoServico? produto = await _produtoRepositorio.ObterPorIdAsync(id, cancellationToken);

        if (produto is null)
        {
            throw new NaoEncontradoException($"O produto {id} não foi encontrado.");
        }

        return produto;
    }
}
