using Franquias.Api.Models;

namespace Franquias.Api.Common;

/// <summary>
/// Nomes dos perfis utilizados nos atributos [Authorize(Roles = ...)].
/// As constantes evitam o uso de "strings mágicas" espalhadas pelos controllers.
/// </summary>
public static class Perfis
{
    public const string Administrador = nameof(PerfilUsuario.Administrador);

    public const string Gestor = nameof(PerfilUsuario.Gestor);

    public const string Operador = nameof(PerfilUsuario.Operador);

    /// <summary>Administrador da franqueadora ou gestor da unidade.</summary>
    public const string AdministradorOuGestor = Administrador + "," + Gestor;

    /// <summary>Qualquer usuário autenticado do sistema.</summary>
    public const string Todos = Administrador + "," + Gestor + "," + Operador;
}

/// <summary>
/// Nomes das claims personalizadas gravadas no token JWT.
/// </summary>
public static class ClaimsPersonalizadas
{
    /// <summary>Identificador da unidade franqueada vinculada ao usuário.</summary>
    public const string UnidadeId = "unidadeId";
}
