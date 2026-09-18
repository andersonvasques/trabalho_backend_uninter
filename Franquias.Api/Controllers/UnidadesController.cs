using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e manutenção das unidades franqueadas da rede.
/// </summary>
[ApiController]
[Route("api/unidades")]
[Produces("application/json")]
[Authorize]
public sealed class UnidadesController : ControllerBase
{
    private readonly IUnidadeService _unidadeService;

    public UnidadesController(IUnidadeService unidadeService)
    {
        _unidadeService = unidadeService;
    }

    /// <summary>
    /// Lista as unidades com filtros, ordenação e paginação.
    /// GET /api/unidades?situacao=Ativa&amp;cidade=Curitiba&amp;pagina=1&amp;tamanhoPagina=10
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<UnidadeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<UnidadeDto>>> Listar(
        [FromQuery] ConsultaUnidadesDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<UnidadeDto> resultado =
            await _unidadeService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta uma unidade pelo identificador.
    /// GET /api/unidades/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UnidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        UnidadeDto unidade = await _unidadeService.ObterPorIdAsync(id, cancellationToken);

        return Ok(unidade);
    }

    /// <summary>
    /// Cadastra uma nova unidade franqueada.
    /// POST /api/unidades
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(UnidadeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UnidadeDto>> Criar(
        [FromBody] CriarUnidadeDto dados,
        CancellationToken cancellationToken)
    {
        UnidadeDto unidade = await _unidadeService.CriarAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, unidade);
    }

    /// <summary>
    /// Atualiza os dados de uma unidade.
    /// PUT /api/unidades/1
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.AdministradorOuGestor)]
    [ProducesResponseType(typeof(UnidadeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UnidadeDto>> Atualizar(
        int id,
        [FromBody] AtualizarUnidadeDto dados,
        CancellationToken cancellationToken)
    {
        UnidadeDto unidade = await _unidadeService.AtualizarAsync(id, dados, cancellationToken);

        return Ok(unidade);
    }

    /// <summary>
    /// Reativa uma unidade.
    /// PATCH /api/unidades/1/ativar
    /// </summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(UnidadeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UnidadeDto>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        UnidadeDto unidade = await _unidadeService.ReativarAsync(id, cancellationToken);

        return Ok(unidade);
    }

    /// <summary>
    /// Inativa a unidade preservando todo o histórico (exclusão lógica).
    /// DELETE /api/unidades/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(UnidadeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UnidadeDto>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        UnidadeDto unidade = await _unidadeService.InativarAsync(id, cancellationToken);

        return Ok(unidade);
    }
}
