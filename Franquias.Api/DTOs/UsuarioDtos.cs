using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados necessários para cadastrar um usuário.
/// </summary>
public sealed class CriarUsuarioDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [EnumDataType(typeof(PerfilUsuario), ErrorMessage = "Perfil inválido.")]
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;

    /// <summary>
    /// Unidade vinculada. Obrigatória para os perfis Gestor e Operador
    /// e não permitida para o perfil Administrador.
    /// </summary>
    public int? UnidadeFranqueadaId { get; set; }
}

/// <summary>
/// Dados alteráveis de um usuário já cadastrado.
/// </summary>
public sealed class AtualizarUsuarioDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [EnumDataType(typeof(PerfilUsuario), ErrorMessage = "Perfil inválido.")]
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;

    public int? UnidadeFranqueadaId { get; set; }
}

/// <summary>
/// Dados para troca de senha do próprio usuário autenticado.
/// </summary>
public sealed class AlterarSenhaDto
{
    [Required(ErrorMessage = "A senha atual é obrigatória.")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova senha deve ter entre 6 e 100 caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;
}

/// <summary>
/// Representação de um usuário devolvida pela API (nunca expõe a senha).
/// </summary>
public sealed class UsuarioDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public PerfilUsuario Perfil { get; set; }

    public int? UnidadeFranqueadaId { get; set; }

    public string? UnidadeNome { get; set; }

    public bool Ativo { get; set; }

    public DateTime CriadoEm { get; set; }

    public DateTime? UltimoAcessoEm { get; set; }

    /// <summary>
    /// Converte a entidade em DTO de saída.
    /// </summary>
    public static UsuarioDto DeModelo(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            UnidadeFranqueadaId = usuario.UnidadeFranqueadaId,
            UnidadeNome = usuario.Unidade?.NomeFantasia,
            Ativo = usuario.Ativo,
            CriadoEm = usuario.CriadoEm,
            UltimoAcessoEm = usuario.UltimoAcessoEm
        };
    }
}

/// <summary>
/// Filtros aceitos na listagem de usuários.
/// </summary>
public sealed class ConsultaUsuariosDto : ParametrosDeConsulta
{
    /// <summary>Texto pesquisado no nome ou no e-mail.</summary>
    public string? Busca { get; set; }

    /// <summary>Filtra pelo perfil de acesso.</summary>
    public PerfilUsuario? Perfil { get; set; }

    /// <summary>Filtra por usuários ativos ou inativos.</summary>
    public bool? Ativo { get; set; }

    /// <summary>Filtra pela unidade vinculada.</summary>
    public int? UnidadeFranqueadaId { get; set; }
}
