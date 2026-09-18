using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro da franqueadora (matriz da rede).
/// </summary>
[ApiController]
[Route("api/franqueadoras")]
[Produces("application/json")]
[Authorize]
public sealed class FranqueadorasController : ControllerBase
{
    private readonly IRedeService _redeService;

    public FranqueadorasController(IRedeService redeService)
    {
        _redeService = redeService;
    }

    /// <summary>
    /// Lista as franqueadoras cadastradas.
    /// GET /api/franqueadoras
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<FranqueadoraDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<FranqueadoraDto>>> Listar(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<FranqueadoraDto> franqueadoras =
            await _redeService.ListarFranqueadorasAsync(cancellationToken);

        return Ok(franqueadoras);
    }

    /// <summary>
    /// Consulta uma franqueadora pelo identificador.
    /// GET /api/franqueadoras/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FranqueadoraDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranqueadoraDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        FranqueadoraDto franqueadora = await _redeService.ObterFranqueadoraAsync(id, cancellationToken);

        return Ok(franqueadora);
    }

    /// <summary>
    /// Cadastra uma nova franqueadora.
    /// POST /api/franqueadoras
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FranqueadoraDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FranqueadoraDto>> Criar(
        [FromBody] FranqueadoraEntradaDto dados,
        CancellationToken cancellationToken)
    {
        FranqueadoraDto franqueadora =
            await _redeService.CriarFranqueadoraAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = franqueadora.Id }, franqueadora);
    }

    /// <summary>
    /// Atualiza os dados da franqueadora.
    /// PUT /api/franqueadoras/1
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FranqueadoraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FranqueadoraDto>> Atualizar(
        int id,
        [FromBody] FranqueadoraEntradaDto dados,
        CancellationToken cancellationToken)
    {
        FranqueadoraDto franqueadora =
            await _redeService.AtualizarFranqueadoraAsync(id, dados, cancellationToken);

        return Ok(franqueadora);
    }
}
