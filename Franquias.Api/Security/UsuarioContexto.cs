using System.Security.Claims;
using Franquias.Api.Common;
using Franquias.Api.Models;

namespace Franquias.Api.Security;

/// <summary>
/// Implementação de <see cref="IUsuarioContexto"/> baseada no
/// <see cref="IHttpContextAccessor"/>, que dá acesso à requisição atual.
/// </summary>
public sealed class UsuarioContexto : IUsuarioContexto
{
    private readonly IHttpContextAccessor _acessor;

    public UsuarioContexto(IHttpContextAccessor acessor)
    {
        _acessor = acessor;
    }

    private ClaimsPrincipal? Usuario => _acessor.HttpContext?.User;

    public int UsuarioId
    {
        get
        {
            string? valor = Usuario?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(valor, out int id))
            {
                return id;
            }

            throw new NaoAutorizadoException("Não foi possível identificar o usuário autenticado.");
        }
    }

    public string Nome => Usuario?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public PerfilUsuario Perfil
    {
        get
        {
            string? valor = Usuario?.FindFirstValue(ClaimTypes.Role);

            if (Enum.TryParse(valor, out PerfilUsuario perfil))
            {
                return perfil;
            }

            throw new NaoAutorizadoException("O perfil do usuário autenticado não foi identificado.");
        }
    }

    public int? UnidadeId
    {
        get
        {
            string? valor = Usuario?.FindFirstValue(ClaimsPersonalizadas.UnidadeId);

            if (int.TryParse(valor, out int id))
            {
                return id;
            }

            return null;
        }
    }

    public bool EhAdministrador => Perfil == PerfilUsuario.Administrador;

    public void GarantirAcessoAUnidade(int unidadeId)
    {
        if (EhAdministrador)
        {
            return;
        }

        if (UnidadeId is null)
        {
            throw new AcessoNegadoException(
                "O usuário não está vinculado a nenhuma unidade franqueada.");
        }

        if (UnidadeId.Value != unidadeId)
        {
            throw new AcessoNegadoException(
                "O usuário só pode acessar os dados da própria unidade franqueada.");
        }
    }

    public int? ResolverFiltroDeUnidade(int? unidadeInformada)
    {
        if (EhAdministrador)
        {
            return unidadeInformada;
        }

        if (UnidadeId is null)
        {
            throw new AcessoNegadoException(
                "O usuário não está vinculado a nenhuma unidade franqueada.");
        }

        if (unidadeInformada.HasValue && unidadeInformada.Value != UnidadeId.Value)
        {
            throw new AcessoNegadoException(
                "O usuário só pode consultar os dados da própria unidade franqueada.");
        }

        return UnidadeId;
    }
}
