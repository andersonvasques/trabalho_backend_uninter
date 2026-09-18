using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Registro e consulta das vendas das unidades franqueadas.
/// </summary>
[ApiController]
[Route("api/vendas")]
[Produces("application/json")]
[Authorize]
public sealed class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    /// <summary>
    /// Lista as vendas por unidade, período, status e forma de pagamento.
    /// GET /api/vendas?unidadeFranqueadaId=1&amp;dataInicial=2026-01-01&amp;dataFinal=2026-03-31
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<VendaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<VendaDto>>> Listar(
        [FromQuery] ConsultaVendasDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<VendaDto> resultado =
            await _vendaService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta uma venda e seus itens.
    /// GET /api/vendas/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        VendaDto venda = await _vendaService.ObterPorIdAsync(id, cancellationToken);

        return Ok(venda);
    }

    /// <summary>
    /// Registra uma venda: calcula o total pelos itens e baixa o estoque.
    /// POST /api/vendas
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<VendaDto>> Registrar(
        [FromBody] CriarVendaDto dados,
        CancellationToken cancellationToken)
    {
        VendaDto venda = await _vendaService.RegistrarAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
    }

    /// <summary>
    /// Cancela a venda, devolve os itens ao estoque e mantém o histórico.
    /// PATCH /api/vendas/1/cancelar
    /// </summary>
    [HttpPatch("{id:int}/cancelar")]
    [Authorize(Roles = Perfis.AdministradorOuGestor)]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VendaDto>> Cancelar(
        int id,
        [FromBody] CancelarVendaDto dados,
        CancellationToken cancellationToken)
    {
        VendaDto venda = await _vendaService.CancelarAsync(id, dados, cancellationToken);

        return Ok(venda);
    }
}
