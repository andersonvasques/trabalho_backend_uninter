using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Relatórios e indicadores gerenciais da rede de franquias.
/// </summary>
[ApiController]
[Route("api/relatorios")]
[Produces("application/json")]
[Authorize]
public sealed class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Faturamento por unidade no período.
    /// GET /api/relatorios/faturamento?dataInicial=2026-01-01&amp;dataFinal=2026-03-31
    /// </summary>
    [HttpGet("faturamento")]
    [ProducesResponseType(typeof(IReadOnlyCollection<FaturamentoPorUnidadeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<FaturamentoPorUnidadeDto>>> ObterFaturamento(
        [FromQuery] ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<FaturamentoPorUnidadeDto> resultado =
            await _relatorioService.ObterFaturamentoAsync(periodo, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Ranking das unidades por faturamento no período.
    /// GET /api/relatorios/ranking-unidades?dataInicial=2026-01-01&amp;dataFinal=2026-03-31&amp;quantidade=10
    /// </summary>
    [HttpGet("ranking-unidades")]
    [ProducesResponseType(typeof(IReadOnlyCollection<RankingUnidadeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RankingUnidadeDto>>> ObterRanking(
        [FromQuery] ConsultaPeriodoDto periodo,
        [FromQuery] int quantidade,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<RankingUnidadeDto> resultado =
            await _relatorioService.ObterRankingAsync(periodo, quantidade, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Produtos e serviços mais vendidos no período.
    /// GET /api/relatorios/produtos-mais-vendidos?dataInicial=2026-01-01&amp;dataFinal=2026-03-31
    /// </summary>
    [HttpGet("produtos-mais-vendidos")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProdutoMaisVendidoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProdutoMaisVendidoDto>>> ObterProdutosMaisVendidos(
        [FromQuery] ConsultaPeriodoDto periodo,
        [FromQuery] int quantidade,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<ProdutoMaisVendidoDto> resultado =
            await _relatorioService.ObterProdutosMaisVendidosAsync(periodo, quantidade, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Itens com estoque igual ou abaixo do mínimo.
    /// GET /api/relatorios/estoque-critico?unidadeId=1
    /// </summary>
    [HttpGet("estoque-critico")]
    [ProducesResponseType(typeof(IReadOnlyCollection<EstoqueCriticoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<EstoqueCriticoDto>>> ObterEstoqueCritico(
        [FromQuery] int? unidadeId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<EstoqueCriticoDto> resultado =
            await _relatorioService.ObterEstoqueCriticoAsync(unidadeId, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Royalties apurados por unidade no período.
    /// GET /api/relatorios/royalties?dataInicial=2026-01-01&amp;dataFinal=2026-03-31
    /// </summary>
    [HttpGet("royalties")]
    [ProducesResponseType(typeof(IReadOnlyCollection<RoyaltiesPorUnidadeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RoyaltiesPorUnidadeDto>>> ObterRoyalties(
        [FromQuery] ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<RoyaltiesPorUnidadeDto> resultado =
            await _relatorioService.ObterRoyaltiesAsync(periodo, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Quantidade de chamados por situação.
    /// GET /api/relatorios/chamados-por-status?unidadeId=1
    /// </summary>
    [HttpGet("chamados-por-status")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ChamadosPorStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ChamadosPorStatusDto>>> ObterChamadosPorStatus(
        [FromQuery] int? unidadeId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<ChamadosPorStatusDto> resultado =
            await _relatorioService.ObterChamadosPorStatusAsync(unidadeId, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Chamados em aberto agrupados por prioridade.
    /// GET /api/relatorios/chamados-por-prioridade?unidadeId=1
    /// </summary>
    [HttpGet("chamados-por-prioridade")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ChamadosPorPrioridadeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ChamadosPorPrioridadeDto>>> ObterChamadosPorPrioridade(
        [FromQuery] int? unidadeId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<ChamadosPorPrioridadeDto> resultado =
            await _relatorioService.ObterChamadosPorPrioridadeAsync(unidadeId, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Painel gerencial com os principais indicadores da rede.
    /// GET /api/relatorios/painel?dataInicial=2026-01-01&amp;dataFinal=2026-03-31
    /// </summary>
    [HttpGet("painel")]
    [ProducesResponseType(typeof(PainelGerencialDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PainelGerencialDto>> ObterPainel(
        [FromQuery] ConsultaPeriodoDto periodo,
        CancellationToken cancellationToken)
    {
        PainelGerencialDto painel = await _relatorioService.ObterPainelAsync(periodo, cancellationToken);

        return Ok(painel);
    }
}
