using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Consultas gerenciais da rede de franquias.
/// </summary>
public interface IRelatorioService
{
    /// <summary>Faturamento por unidade dentro do período informado.</summary>
    Task<IReadOnlyCollection<FaturamentoPorUnidadeDto>> ObterFaturamentoAsync(
        ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken);

    /// <summary>Ranking das unidades por faturamento.</summary>
    Task<IReadOnlyCollection<RankingUnidadeDto>> ObterRankingAsync(
        ConsultaPeriodoDto periodo,
        int quantidade,
        CancellationToken cancellationToken);

    /// <summary>Produtos e serviços mais vendidos no período.</summary>
    Task<IReadOnlyCollection<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(
        ConsultaPeriodoDto periodo,
        int quantidade,
        CancellationToken cancellationToken);

    /// <summary>Itens com estoque igual ou abaixo do mínimo.</summary>
    Task<IReadOnlyCollection<EstoqueCriticoDto>> ObterEstoqueCriticoAsync(
        int? unidadeId,
        CancellationToken cancellationToken);

    /// <summary>Royalties apurados por unidade no período.</summary>
    Task<IReadOnlyCollection<RoyaltiesPorUnidadeDto>> ObterRoyaltiesAsync(
        ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken);

    /// <summary>Quantidade de chamados por situação.</summary>
    Task<IReadOnlyCollection<ChamadosPorStatusDto>> ObterChamadosPorStatusAsync(
        int? unidadeId,
        CancellationToken cancellationToken);

    /// <summary>Quantidade de chamados por prioridade.</summary>
    Task<IReadOnlyCollection<ChamadosPorPrioridadeDto>> ObterChamadosPorPrioridadeAsync(
        int? unidadeId,
        CancellationToken cancellationToken);

    /// <summary>Painel com os principais indicadores da rede.</summary>
    Task<PainelGerencialDto> ObterPainelAsync(
        ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken);
}
