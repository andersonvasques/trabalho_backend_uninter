using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das consultas gerenciais.
///
/// Os dados são lidos do banco já filtrados pelo período e, em seguida,
/// agrupados com LINQ para produzir os indicadores.
/// </summary>
public sealed class RelatorioService : IRelatorioService
{
    private readonly IRelatorioRepositorio _relatorioRepositorio;
    private readonly IEstoqueRepositorio _estoqueRepositorio;
    private readonly IRoyaltyRepositorio _royaltyRepositorio;
    private readonly IChamadoRepositorio _chamadoRepositorio;
    private readonly IUsuarioContexto _usuarioContexto;

    public RelatorioService(
        IRelatorioRepositorio relatorioRepositorio,
        IEstoqueRepositorio estoqueRepositorio,
        IRoyaltyRepositorio royaltyRepositorio,
        IChamadoRepositorio chamadoRepositorio,
        IUsuarioContexto usuarioContexto)
    {
        _relatorioRepositorio = relatorioRepositorio;
        _estoqueRepositorio = estoqueRepositorio;
        _royaltyRepositorio = royaltyRepositorio;
        _chamadoRepositorio = chamadoRepositorio;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<IReadOnlyCollection<FaturamentoPorUnidadeDto>> ObterFaturamentoAsync(
        ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken)
    {
        (DateTime inicio, DateTime fim, int? unidadeId) = PrepararPeriodo(periodo);

        IReadOnlyCollection<VendaConsolidada> vendas =
            await _relatorioRepositorio.ListarVendasConfirmadasAsync(
                unidadeId,
                inicio,
                fim,
                cancellationToken);

        List<FaturamentoPorUnidadeDto> resultado = vendas
            .GroupBy(venda => new
            {
                venda.UnidadeFranqueadaId,
                venda.UnidadeNome,
                venda.Cidade,
                venda.Situacao
            })
            .Select(grupo => new FaturamentoPorUnidadeDto
            {
                UnidadeFranqueadaId = grupo.Key.UnidadeFranqueadaId,
                UnidadeNome = grupo.Key.UnidadeNome,
                Cidade = grupo.Key.Cidade,
                Situacao = grupo.Key.Situacao,
                QuantidadeDeVendas = grupo.Count(),
                Faturamento = decimal.Round(grupo.Sum(venda => venda.ValorTotal), 2),
                TicketMedio = grupo.Count() == 0
                    ? 0m
                    : decimal.Round(grupo.Sum(venda => venda.ValorTotal) / grupo.Count(), 2)
            })
            .OrderByDescending(item => item.Faturamento)
            .ToList();

        return resultado;
    }

    public async Task<IReadOnlyCollection<RankingUnidadeDto>> ObterRankingAsync(
        ConsultaPeriodoDto periodo,
        int quantidade,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<FaturamentoPorUnidadeDto> faturamentos =
            await ObterFaturamentoAsync(periodo, cancellationToken);

        decimal total = 0m;

        foreach (FaturamentoPorUnidadeDto item in faturamentos)
        {
            total += item.Faturamento;
        }

        int limite = quantidade <= 0 ? 10 : quantidade;
        var ranking = new List<RankingUnidadeDto>();
        int posicao = 1;

        foreach (FaturamentoPorUnidadeDto item in faturamentos.Take(limite))
        {
            ranking.Add(new RankingUnidadeDto
            {
                Posicao = posicao,
                UnidadeFranqueadaId = item.UnidadeFranqueadaId,
                UnidadeNome = item.UnidadeNome,
                Cidade = item.Cidade,
                QuantidadeDeVendas = item.QuantidadeDeVendas,
                Faturamento = item.Faturamento,
                ParticipacaoPercentual = total == 0m
                    ? 0m
                    : decimal.Round((item.Faturamento / total) * 100m, 2)
            });

            posicao++;
        }

        return ranking;
    }

    public async Task<IReadOnlyCollection<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(
        ConsultaPeriodoDto periodo,
        int quantidade,
        CancellationToken cancellationToken)
    {
        (DateTime inicio, DateTime fim, int? unidadeId) = PrepararPeriodo(periodo);

        IReadOnlyCollection<ItemVendidoConsolidado> itens =
            await _relatorioRepositorio.ListarItensVendidosAsync(
                unidadeId,
                inicio,
                fim,
                cancellationToken);

        int limite = quantidade <= 0 ? 10 : quantidade;

        return itens
            .GroupBy(item => new { item.ProdutoServicoId, item.Sku, item.Nome, item.Categoria })
            .Select(grupo => new ProdutoMaisVendidoDto
            {
                ProdutoServicoId = grupo.Key.ProdutoServicoId,
                Sku = grupo.Key.Sku,
                Nome = grupo.Key.Nome,
                Categoria = grupo.Key.Categoria,
                QuantidadeVendida = grupo.Sum(item => item.Quantidade),
                ValorTotal = decimal.Round(grupo.Sum(item => item.Subtotal), 2)
            })
            .OrderByDescending(item => item.QuantidadeVendida)
            .ThenByDescending(item => item.ValorTotal)
            .Take(limite)
            .ToList();
    }

    public async Task<IReadOnlyCollection<EstoqueCriticoDto>> ObterEstoqueCriticoAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        int? filtro = _usuarioContexto.ResolverFiltroDeUnidade(unidadeId);

        IReadOnlyCollection<EstoqueUnidade> estoques =
            await _estoqueRepositorio.ListarAbaixoDoMinimoAsync(filtro, cancellationToken);

        var resultado = new List<EstoqueCriticoDto>();

        foreach (EstoqueUnidade estoque in estoques)
        {
            resultado.Add(new EstoqueCriticoDto
            {
                UnidadeFranqueadaId = estoque.UnidadeFranqueadaId,
                UnidadeNome = estoque.Unidade?.NomeFantasia ?? string.Empty,
                ProdutoServicoId = estoque.ProdutoServicoId,
                ProdutoSku = estoque.ProdutoServico?.Sku ?? string.Empty,
                ProdutoNome = estoque.ProdutoServico?.Nome ?? string.Empty,
                Quantidade = estoque.Quantidade,
                QuantidadeMinima = estoque.QuantidadeMinima,
                QuantidadeParaRepor = estoque.QuantidadeMinima - estoque.Quantidade < 0
                    ? 0
                    : estoque.QuantidadeMinima - estoque.Quantidade
            });
        }

        return resultado;
    }

    public async Task<IReadOnlyCollection<RoyaltiesPorUnidadeDto>> ObterRoyaltiesAsync(
        ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken)
    {
        (DateTime inicio, DateTime fim, int? unidadeId) = PrepararPeriodo(periodo);

        IReadOnlyCollection<Royalty> royalties = await _royaltyRepositorio.ListarPorPeriodoAsync(
            unidadeId,
            inicio,
            fim,
            cancellationToken);

        return royalties
            .GroupBy(royalty => new
            {
                royalty.UnidadeFranqueadaId,
                Nome = royalty.Unidade != null ? royalty.Unidade.NomeFantasia : string.Empty
            })
            .Select(grupo => new RoyaltiesPorUnidadeDto
            {
                UnidadeFranqueadaId = grupo.Key.UnidadeFranqueadaId,
                UnidadeNome = grupo.Key.Nome,
                FaturamentoBase = decimal.Round(grupo.Sum(royalty => royalty.FaturamentoBase), 2),
                TotalDevido = decimal.Round(grupo.Sum(royalty => royalty.ValorDevido), 2),
                TotalPago = decimal.Round(grupo.Sum(royalty => royalty.ValorPago), 2),
                SaldoDevedor = decimal.Round(grupo.Sum(royalty => royalty.ObterSaldoDevedor()), 2),
                QuantidadeDeCobrancas = grupo.Count()
            })
            .OrderByDescending(item => item.TotalDevido)
            .ToList();
    }

    public async Task<IReadOnlyCollection<ChamadosPorStatusDto>> ObterChamadosPorStatusAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        int? filtro = _usuarioContexto.ResolverFiltroDeUnidade(unidadeId);

        IReadOnlyCollection<ChamadoSuporte> chamados =
            await _chamadoRepositorio.ListarParaIndicadoresAsync(filtro, cancellationToken);

        return chamados
            .GroupBy(chamado => chamado.Status)
            .Select(grupo => new ChamadosPorStatusDto
            {
                Status = grupo.Key,
                Quantidade = grupo.Count()
            })
            .OrderBy(item => item.Status)
            .ToList();
    }

    public async Task<IReadOnlyCollection<ChamadosPorPrioridadeDto>> ObterChamadosPorPrioridadeAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        int? filtro = _usuarioContexto.ResolverFiltroDeUnidade(unidadeId);

        IReadOnlyCollection<ChamadoSuporte> chamados =
            await _chamadoRepositorio.ListarParaIndicadoresAsync(filtro, cancellationToken);

        return chamados
            .Where(chamado => chamado.EstaEmAberto())
            .GroupBy(chamado => chamado.Prioridade)
            .Select(grupo => new ChamadosPorPrioridadeDto
            {
                Prioridade = grupo.Key,
                Quantidade = grupo.Count()
            })
            .OrderByDescending(item => item.Prioridade)
            .ToList();
    }

    public async Task<PainelGerencialDto> ObterPainelAsync(
        ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken)
    {
        (DateTime inicio, DateTime fim, int? unidadeId) = PrepararPeriodo(periodo);

        IReadOnlyCollection<UnidadeFranqueada> unidades =
            await _relatorioRepositorio.ListarUnidadesAsync(unidadeId, cancellationToken);

        IReadOnlyCollection<FaturamentoPorUnidadeDto> faturamentos =
            await ObterFaturamentoAsync(periodo, cancellationToken);

        IReadOnlyCollection<RankingUnidadeDto> ranking =
            await ObterRankingAsync(periodo, 5, cancellationToken);

        IReadOnlyCollection<ProdutoMaisVendidoDto> produtos =
            await ObterProdutosMaisVendidosAsync(periodo, 5, cancellationToken);

        IReadOnlyCollection<RoyaltiesPorUnidadeDto> royalties =
            await ObterRoyaltiesAsync(periodo, cancellationToken);

        IReadOnlyCollection<EstoqueCriticoDto> estoqueCritico =
            await ObterEstoqueCriticoAsync(periodo.UnidadeFranqueadaId, cancellationToken);

        IReadOnlyCollection<ChamadosPorStatusDto> chamados =
            await ObterChamadosPorStatusAsync(periodo.UnidadeFranqueadaId, cancellationToken);

        int quantidadeDeVendas = faturamentos.Sum(item => item.QuantidadeDeVendas);
        decimal faturamentoTotal = decimal.Round(faturamentos.Sum(item => item.Faturamento), 2);

        int chamadosEmAberto = chamados
            .Where(item => item.Status == StatusChamado.Aberto || item.Status == StatusChamado.EmAtendimento)
            .Sum(item => item.Quantidade);

        return new PainelGerencialDto
        {
            DataInicial = inicio,
            DataFinal = fim,
            TotalDeUnidades = unidades.Count,
            UnidadesAtivas = unidades.Count(unidade => unidade.Situacao == SituacaoUnidade.Ativa),
            UnidadesInativas = unidades.Count(unidade => unidade.Situacao == SituacaoUnidade.Inativa),
            QuantidadeDeVendas = quantidadeDeVendas,
            FaturamentoTotal = faturamentoTotal,
            TicketMedio = quantidadeDeVendas == 0
                ? 0m
                : decimal.Round(faturamentoTotal / quantidadeDeVendas, 2),
            RoyaltiesDevidos = decimal.Round(royalties.Sum(item => item.TotalDevido), 2),
            RoyaltiesPagos = decimal.Round(royalties.Sum(item => item.TotalPago), 2),
            ItensComEstoqueCritico = estoqueCritico.Count,
            ChamadosEmAberto = chamadosEmAberto,
            TopUnidades = ranking,
            TopProdutos = produtos
        };
    }

    /// <summary>
    /// Valida o período informado e aplica o filtro de unidade conforme o perfil.
    /// </summary>
    private (DateTime Inicio, DateTime Fim, int? UnidadeId) PrepararPeriodo(ConsultaPeriodoDto periodo)
    {
        if (periodo.DataFinal.Date < periodo.DataInicial.Date)
        {
            throw new RegraDeNegocioException(
                "A data final do período deve ser maior ou igual à data inicial.");
        }

        DateTime inicio = periodo.DataInicial.Date;
        DateTime fim = periodo.DataFinal.Date.AddDays(1).AddTicks(-1);
        int? unidadeId = _usuarioContexto.ResolverFiltroDeUnidade(periodo.UnidadeFranqueadaId);

        return (inicio, fim, unidadeId);
    }
}
