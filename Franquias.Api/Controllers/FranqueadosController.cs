using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro dos franqueados responsáveis pelas unidades.
/// </summary>
[ApiController]
[Route("api/franqueados")]
[Produces("application/json")]
[Authorize(Roles = Perfis.AdministradorOuGestor)]
public sealed class FranqueadosController : ControllerBase
{
    private readonly IRedeService _redeService;

    public FranqueadosController(IRedeService redeService)
    {
        _redeService = redeService;
    }

    /// <summary>
    /// Lista os franqueados com busca e paginação.
    /// GET /api/franqueados?busca=silva&amp;ativo=true
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<FranqueadoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<FranqueadoDto>>> Listar(
        [FromQuery] ConsultaFranqueadosDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<FranqueadoDto> resultado =
            await _redeService.ListarFranqueadosAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta um franqueado pelo identificador.
    /// GET /api/franqueados/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FranqueadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranqueadoDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        FranqueadoDto franqueado = await _redeService.ObterFranqueadoAsync(id, cancellationToken);

        return Ok(franqueado);
    }

    /// <summary>
    /// Cadastra um novo franqueado.
    /// POST /api/franqueados
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FranqueadoDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FranqueadoDto>> Criar(
        [FromBody] FranqueadoEntradaDto dados,
        CancellationToken cancellationToken)
    {
        FranqueadoDto franqueado = await _redeService.CriarFranqueadoAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = franqueado.Id }, franqueado);
    }

    /// <summary>
    /// Atualiza os dados de um franqueado.
    /// PUT /api/franqueados/1
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FranqueadoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FranqueadoDto>> Atualizar(
        int id,
        [FromBody] FranqueadoEntradaDto dados,
        CancellationToken cancellationToken)
    {
        FranqueadoDto franqueado =
            await _redeService.AtualizarFranqueadoAsync(id, dados, cancellationToken);

        return Ok(franqueado);
    }

    /// <summary>
    /// Reativa um franqueado.
    /// PATCH /api/franqueados/1/ativar
    /// </summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FranqueadoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FranqueadoDto>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        FranqueadoDto franqueado =
            await _redeService.AlterarSituacaoFranqueadoAsync(id, true, cancellationToken);

        return Ok(franqueado);
    }

    /// <summary>
    /// Inativa o franqueado (exclusão lógica).
    /// DELETE /api/franqueados/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(FranqueadoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FranqueadoDto>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        FranqueadoDto franqueado =
            await _redeService.AlterarSituacaoFranqueadoAsync(id, false, cancellationToken);

        return Ok(franqueado);
    }
}
