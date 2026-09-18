using Franquias.Api.Models;

namespace Franquias.Api.Security;

/// <summary>
/// Dados do token gerado para o usuário autenticado.
/// </summary>
/// <param name="Token">Token JWT assinado.</param>
/// <param name="ExpiraEm">Data e hora (UTC) de expiração.</param>
public sealed record TokenGerado(string Token, DateTime ExpiraEm);

/// <summary>
/// Contrato do componente responsável por emitir tokens JWT.
/// </summary>
public interface IGeradorDeToken
{
    /// <summary>
    /// Gera um token JWT contendo o identificador, o perfil e a unidade do usuário.
    /// </summary>
    TokenGerado Gerar(Usuario usuario);
}
