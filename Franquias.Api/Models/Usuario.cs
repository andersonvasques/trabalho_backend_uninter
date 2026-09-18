namespace Franquias.Api.Models;

/// <summary>
/// Usuário do sistema. Pode pertencer à franqueadora (administrador)
/// ou a uma unidade franqueada (gestor ou operador).
/// </summary>
public sealed class Usuario
{
    /// <summary>Identificador único do usuário.</summary>
    public int Id { get; set; }

    /// <summary>Nome completo do usuário.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail utilizado no login. Não pode se repetir.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash da senha (PBKDF2 + SHA256) em Base64.</summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>Salt aleatório utilizado na geração do hash, em Base64.</summary>
    public string SenhaSalt { get; set; } = string.Empty;

    /// <summary>Perfil de acesso do usuário.</summary>
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;

    /// <summary>
    /// Unidade à qual o usuário está vinculado.
    /// Fica nulo para usuários da franqueadora (administradores).
    /// </summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>Unidade vinculada ao usuário.</summary>
    public UnidadeFranqueada? Unidade { get; set; }

    /// <summary>Indica se o usuário pode acessar o sistema.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Data de criação do cadastro.</summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>Data da última alteração do cadastro.</summary>
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Data do último login bem-sucedido.</summary>
    public DateTime? UltimoAcessoEm { get; set; }

    /// <summary>
    /// Indica se o usuário enxerga toda a rede.
    /// </summary>
    public bool EhDaFranqueadora()
    {
        return Perfil == PerfilUsuario.Administrador;
    }
}
