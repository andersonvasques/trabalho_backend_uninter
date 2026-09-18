using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Categorias do catálogo padronizado da rede.
/// </summary>
[ApiController]
[Route("api/categorias")]
[Produces("application/json")]
[Authorize]
public sealed class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// <summary>
    /// Lista as categorias.
    /// GET /api/categorias?apenasAtivas=true
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CategoriaDto>>> Listar(
        [FromQuery] bool? apenasAtivas,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<CategoriaDto> categorias =
            await _categoriaService.ListarAsync(apenasAtivas, cancellationToken);

        return Ok(categorias);
    }

    /// <summary>
    /// Consulta uma categoria pelo identificador.
    /// GET /api/categorias/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        CategoriaDto categoria = await _categoriaService.ObterPorIdAsync(id, cancellationToken);

        return Ok(categoria);
    }

    /// <summary>
    /// Cadastra uma nova categoria.
    /// POST /api/categorias
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CategoriaDto>> Criar(
        [FromBody] CategoriaEntradaDto dados,
        CancellationToken cancellationToken)
    {
        CategoriaDto categoria = await _categoriaService.CriarAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
    }

    /// <summary>
    /// Atualiza uma categoria.
    /// PUT /api/categorias/1
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoriaDto>> Atualizar(
        int id,
        [FromBody] CategoriaEntradaDto dados,
        CancellationToken cancellationToken)
    {
        CategoriaDto categoria = await _categoriaService.AtualizarAsync(id, dados, cancellationToken);

        return Ok(categoria);
    }

    /// <summary>
    /// Reativa uma categoria.
    /// PATCH /api/categorias/1/ativar
    /// </summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoriaDto>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        CategoriaDto categoria = await _categoriaService.AlterarSituacaoAsync(id, true, cancellationToken);

        return Ok(categoria);
    }

    /// <summary>
    /// Inativa a categoria (exclusão lógica).
    /// DELETE /api/categorias/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoriaDto>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        CategoriaDto categoria = await _categoriaService.AlterarSituacaoAsync(id, false, cancellationToken);

        return Ok(categoria);
    }
}
