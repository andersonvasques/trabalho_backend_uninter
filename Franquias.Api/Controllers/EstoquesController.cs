using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Controle do estoque de cada unidade franqueada.
/// </summary>
[ApiController]
[Route("api/estoques")]
[Produces("application/json")]
[Authorize]
public sealed class EstoquesController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public EstoquesController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    /// <summary>
    /// Consulta os saldos de estoque com filtros e paginação.
    /// GET /api/estoques?unidadeFranqueadaId=1&amp;apenasAbaixoDoMinimo=false
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<EstoqueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<EstoqueDto>>> Listar(
        [FromQuery] ConsultaEstoqueDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<EstoqueDto> resultado =
            await _estoqueService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta o saldo de um produto em uma unidade.
    /// GET /api/estoques/unidades/1/produtos/5
    /// </summary>
    [HttpGet("unidades/{unidadeId:int}/produtos/{produtoId:int}")]
    [ProducesResponseType(typeof(EstoqueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstoqueDto>> ObterSaldo(
        int unidadeId,
        int produtoId,
        CancellationToken cancellationToken)
    {
        EstoqueDto estoque = await _estoqueService.ObterSaldoAsync(
            unidadeId,
            produtoId,
            cancellationToken);

        return Ok(estoque);
    }

    /// <summary>
    /// Lista os itens com saldo igual ou abaixo do estoque mínimo.
    /// GET /api/estoques/criticos?unidadeFranqueadaId=1
    /// </summary>
    [HttpGet("criticos")]
    [ProducesResponseType(typeof(IReadOnlyCollection<EstoqueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<EstoqueDto>>> ListarCriticos(
        [FromQuery] int? unidadeFranqueadaId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<EstoqueDto> itens =
            await _estoqueService.ListarAbaixoDoMinimoAsync(unidadeFranqueadaId, cancellationToken);

        return Ok(itens);
    }

    /// <summary>
    /// Registra uma entrada, saída ou ajuste de estoque.
    /// O saldo nunca pode ficar negativo.
    /// POST /api/estoques/movimentacoes
    /// </summary>
    [HttpPost("movimentacoes")]
    [ProducesResponseType(typeof(EstoqueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstoqueDto>> Movimentar(
        [FromBody] MovimentarEstoqueDto dados,
        CancellationToken cancellationToken)
    {
        EstoqueDto estoque = await _estoqueService.MovimentarAsync(dados, cancellationToken);

        return Ok(estoque);
    }

    /// <summary>
    /// Consulta o histórico de movimentações do estoque.
    /// GET /api/estoques/movimentacoes?unidadeFranqueadaId=1&amp;tipo=Saida
    /// </summary>
    [HttpGet("movimentacoes")]
    [ProducesResponseType(typeof(ResultadoPaginado<MovimentacaoEstoqueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<MovimentacaoEstoqueDto>>> ListarMovimentacoes(
        [FromQuery] ConsultaMovimentacoesDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<MovimentacaoEstoqueDto> resultado =
            await _estoqueService.ListarMovimentacoesAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Define o estoque mínimo de um produto na unidade.
    /// PUT /api/estoques/minimo
    /// </summary>
    [HttpPut("minimo")]
    [Authorize(Roles = Perfis.AdministradorOuGestor)]
    [ProducesResponseType(typeof(EstoqueDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EstoqueDto>> DefinirEstoqueMinimo(
        [FromBody] DefinirEstoqueMinimoDto dados,
        CancellationToken cancellationToken)
    {
        EstoqueDto estoque = await _estoqueService.DefinirEstoqueMinimoAsync(dados, cancellationToken);

        return Ok(estoque);
    }
}
