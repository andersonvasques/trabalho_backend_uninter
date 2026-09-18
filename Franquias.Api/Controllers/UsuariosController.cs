using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro dos usuários do sistema e controle de perfis de acesso.
/// </summary>
[ApiController]
[Route("api/usuarios")]
[Produces("application/json")]
[Authorize(Roles = Perfis.AdministradorOuGestor)]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Lista os usuários com filtros, ordenação e paginação.
    /// GET /api/usuarios?busca=maria&amp;perfil=Gestor&amp;pagina=1&amp;tamanhoPagina=10
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<UsuarioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<UsuarioDto>>> Listar(
        [FromQuery] ConsultaUsuariosDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<UsuarioDto> resultado =
            await _usuarioService.ListarAsync(filtros, cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Consulta um usuário pelo identificador.
    /// GET /api/usuarios/1
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioDto>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        UsuarioDto usuario = await _usuarioService.ObterPorIdAsync(id, cancellationToken);

        return Ok(usuario);
    }

    /// <summary>
    /// Cadastra um novo usuário. Apenas administradores da franqueadora.
    /// POST /api/usuarios
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioDto>> Criar(
        [FromBody] CriarUsuarioDto dados,
        CancellationToken cancellationToken)
    {
        UsuarioDto usuario = await _usuarioService.CriarAsync(dados, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    /// <summary>
    /// Atualiza os dados de um usuário.
    /// PUT /api/usuarios/1
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioDto>> Atualizar(
        int id,
        [FromBody] AtualizarUsuarioDto dados,
        CancellationToken cancellationToken)
    {
        UsuarioDto usuario = await _usuarioService.AtualizarAsync(id, dados, cancellationToken);

        return Ok(usuario);
    }

    /// <summary>
    /// Ativa um usuário inativo.
    /// PATCH /api/usuarios/1/ativar
    /// </summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioDto>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        UsuarioDto usuario = await _usuarioService.AlterarSituacaoAsync(id, true, cancellationToken);

        return Ok(usuario);
    }

    /// <summary>
    /// Inativa o usuário (exclusão lógica, preservando o histórico).
    /// DELETE /api/usuarios/1
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Administrador)]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioDto>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        UsuarioDto usuario = await _usuarioService.AlterarSituacaoAsync(id, false, cancellationToken);

        return Ok(usuario);
    }
}
