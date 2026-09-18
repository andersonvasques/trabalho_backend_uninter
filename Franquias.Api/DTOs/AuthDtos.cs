using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados enviados no login.
/// </summary>
public sealed class LoginDto
{
    /// <summary>E-mail cadastrado do usuário.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Senha do usuário.</summary>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// Resposta devolvida após um login bem-sucedido.
/// </summary>
public sealed class LoginRespostaDto
{
    /// <summary>Token JWT que deve ser enviado no cabeçalho Authorization.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Tipo do token (sempre "Bearer").</summary>
    public string TipoToken { get; set; } = "Bearer";

    /// <summary>Data e hora (UTC) em que o token expira.</summary>
    public DateTime ExpiraEm { get; set; }

    /// <summary>Dados do usuário autenticado.</summary>
    public UsuarioDto Usuario { get; set; } = null!;
}
