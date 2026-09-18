using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Autenticação dos usuários do sistema.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AuthController(IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    /// <summary>
    /// Realiza o login e devolve o token JWT.
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginRespostaDto>> Login(
        [FromBody] LoginDto dados,
        CancellationToken cancellationToken)
    {
        LoginRespostaDto resposta = await _autenticacaoService.AutenticarAsync(dados, cancellationToken);

        return Ok(resposta);
    }

    /// <summary>
    /// Devolve os dados do usuário autenticado.
    /// GET /api/auth/eu
    /// </summary>
    [HttpGet("eu")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioDto>> ObterUsuarioAutenticado(
        CancellationToken cancellationToken)
    {
        UsuarioDto usuario = await _autenticacaoService.ObterUsuarioAutenticadoAsync(cancellationToken);

        return Ok(usuario);
    }

    /// <summary>
    /// Altera a senha do usuário autenticado.
    /// PUT /api/auth/senha
    /// </summary>
    [HttpPut("senha")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarSenha(
        [FromBody] AlterarSenhaDto dados,
        CancellationToken cancellationToken)
    {
        await _autenticacaoService.AlterarSenhaAsync(dados, cancellationToken);

        return NoContent();
    }
}
