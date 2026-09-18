using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de autenticação do sistema.
/// </summary>
public interface IAutenticacaoService
{
    /// <summary>
    /// Valida as credenciais e devolve o token JWT do usuário.
    /// </summary>
    Task<LoginRespostaDto> AutenticarAsync(LoginDto dados, CancellationToken cancellationToken);

    /// <summary>
    /// Devolve os dados do usuário autenticado na requisição atual.
    /// </summary>
    Task<UsuarioDto> ObterUsuarioAutenticadoAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Troca a senha do usuário autenticado.
    /// </summary>
    Task AlterarSenhaAsync(AlterarSenhaDto dados, CancellationToken cancellationToken);
}
