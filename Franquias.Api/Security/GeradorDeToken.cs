using System.Security.Claims;
using System.Text;
using Franquias.Api.Common;
using Franquias.Api.Configurations;
using Franquias.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api.Security;

/// <summary>
/// Implementação do gerador de tokens JWT.
///
/// As claims gravadas no token são utilizadas depois pela API para
/// identificar o usuário, validar o perfil e limitar o acesso à unidade.
/// </summary>
public sealed class GeradorDeToken : IGeradorDeToken
{
    private readonly JwtOptions _opcoes;

    public GeradorDeToken(IOptions<JwtOptions> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public TokenGerado Gerar(Usuario usuario)
    {
        DateTime expiraEm = DateTime.UtcNow.AddMinutes(_opcoes.ExpiracaoEmMinutos);

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.Chave));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        if (usuario.UnidadeFranqueadaId.HasValue)
        {
            claims.Add(new Claim(
                ClaimsPersonalizadas.UnidadeId,
                usuario.UnidadeFranqueadaId.Value.ToString()));
        }

        var descritor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiraEm,
            Issuer = _opcoes.Emissor,
            Audience = _opcoes.Audiencia,
            SigningCredentials = credenciais
        };

        var manipulador = new JsonWebTokenHandler();
        string token = manipulador.CreateToken(descritor);

        return new TokenGerado(token, expiraEm);
    }
}
