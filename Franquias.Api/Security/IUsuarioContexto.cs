using Franquias.Api.Models;

namespace Franquias.Api.Security;

/// <summary>
/// Expõe, para as camadas de serviço, os dados do usuário autenticado
/// na requisição atual (lidos das claims do token JWT).
/// </summary>
public interface IUsuarioContexto
{
    /// <summary>Identificador do usuário autenticado.</summary>
    int UsuarioId { get; }

    /// <summary>Nome do usuário autenticado.</summary>
    string Nome { get; }

    /// <summary>Perfil de acesso do usuário autenticado.</summary>
    PerfilUsuario Perfil { get; }

    /// <summary>Unidade vinculada ao usuário, quando houver.</summary>
    int? UnidadeId { get; }

    /// <summary>Indica se o usuário pertence à franqueadora.</summary>
    bool EhAdministrador { get; }

    /// <summary>
    /// Garante que o usuário pode operar sobre a unidade informada.
    /// Administradores acessam qualquer unidade; os demais perfis
    /// acessam somente a unidade à qual estão vinculados.
    /// </summary>
    void GarantirAcessoAUnidade(int unidadeId);

    /// <summary>
    /// Retorna a unidade que deve ser usada como filtro nas consultas.
    /// Para administradores devolve o filtro informado na requisição;
    /// para os demais perfis força a unidade do próprio usuário.
    /// </summary>
    int? ResolverFiltroDeUnidade(int? unidadeInformada);
}
