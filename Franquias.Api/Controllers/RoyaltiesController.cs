using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Apuração e cobrança dos royalties das unidades franqueadas.
/// </summary>
[ApiController]
[Route("api/royalties")]
[Produces("application/json")]
[Authorize]
public sealed class RoyaltiesController : ControllerBase
{
    private readonly IRoyaltyService _royaltyService;

    public RoyaltiesController(IRoyaltyService royaltyService)
    {
        _royaltyService = royaltyService;
    }

    /// <summary>
    /// Lista as cobranças por unidade, competência e situação.
    /// GET /api/royalties?ano=2026&amp;mes=8&amp;situacao=Pendente
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<RoyaltyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<RoyaltyDto>>> Listar(
        [FromQuery] ConsultaRoyaltiesDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<RoyaltyDto> resultado =
            await _royaltyService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta uma cobrança pelo identificador.
    /// GET /api/royalties/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RoyaltyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoyaltyDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        RoyaltyDto royalty = await _royaltyService.ObterPorIdAsync(id, cancellationToken);

        return Ok(royalty);
    }

    /// <summary>
    /// Apura os royalties de uma competência com base no faturamento confirmado.
    /// POST /api/royalties/apuracoes
    /// </summary>
    [HttpPost("apuracoes")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(IReadOnlyCollection<RoyaltyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<RoyaltyDto>>> Apurar(
        [FromBody] GerarRoyaltiesDto dados,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<RoyaltyDto> royalties =
            await _royaltyService.ApurarAsync(dados, cancellationToken);

        return Ok(royalties);
    }

    /// <summary>
    /// Registra o pagamento (total ou parcial) de uma cobrança.
    /// PUT /api/royalties/1/pagamento
    /// </summary>
    [HttpPut("{id:int}/pagamento")]
    [Authorize(Roles = Perfis.AdministradorOuGestor)]
    [ProducesResponseType(typeof(RoyaltyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RoyaltyDto>> RegistrarPagamento(
        int id,
        [FromBody] RegistrarPagamentoRoyaltyDto dados,
        CancellationToken cancellationToken)
    {
        RoyaltyDto royalty =
            await _royaltyService.RegistrarPagamentoAsync(id, dados, cancellationToken);

        return Ok(royalty);
    }

    /// <summary>
    /// Cancela uma cobrança pendente, mantendo o histórico.
    /// DELETE /api/royalties/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(RoyaltyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RoyaltyDto>> Cancelar(
        int id,
        CancellationToken cancellationToken)
    {
        RoyaltyDto royalty = await _royaltyService.CancelarAsync(id, cancellationToken);

        return Ok(royalty);
    }
}
