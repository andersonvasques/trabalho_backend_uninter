using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;
using Franquias.Api.Validations;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados de cadastro e alteração da franqueadora.
/// </summary>
public sealed class FranqueadoraEntradaDto
{
    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "A razão social deve ter entre 3 e 150 caracteres.")]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome fantasia deve ter entre 2 e 150 caracteres.")]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [Cnpj]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [UfOpcional]
    public string Uf { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "O percentual de royalty deve estar entre 0 e 100.")]
    public decimal PercentualRoyaltyPadrao { get; set; }

    public DateTime DataFundacao { get; set; }
}

/// <summary>
/// Representação da franqueadora devolvida pela API.
/// </summary>
public sealed class FranqueadoraDto
{
    public int Id { get; set; }

    public string RazaoSocial { get; set; } = string.Empty;

    public string NomeFantasia { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Uf { get; set; } = string.Empty;

    public decimal PercentualRoyaltyPadrao { get; set; }

    public DateTime DataFundacao { get; set; }

    public bool Ativa { get; set; }

    public int QuantidadeDeUnidades { get; set; }

    public static FranqueadoraDto DeModelo(Franqueadora franqueadora, int quantidadeDeUnidades = 0)
    {
        return new FranqueadoraDto
        {
            Id = franqueadora.Id,
            RazaoSocial = franqueadora.RazaoSocial,
            NomeFantasia = franqueadora.NomeFantasia,
            Cnpj = franqueadora.Cnpj,
            Email = franqueadora.Email,
            Telefone = franqueadora.Telefone,
            Cidade = franqueadora.Cidade,
            Uf = franqueadora.Uf,
            PercentualRoyaltyPadrao = franqueadora.PercentualRoyaltyPadrao,
            DataFundacao = franqueadora.DataFundacao,
            Ativa = franqueadora.Ativa,
            QuantidadeDeUnidades = quantidadeDeUnidades
        };
    }
}

/// <summary>
/// Dados de cadastro e alteração de um franqueado.
/// </summary>
public sealed class FranqueadoEntradaDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [Cpf]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [UfOpcional]
    public string Uf { get; set; } = string.Empty;

    public DateTime DataEntradaNaRede { get; set; }
}

/// <summary>
/// Representação de um franqueado devolvida pela API.
/// </summary>
public sealed class FranqueadoDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Uf { get; set; } = string.Empty;

    public DateTime DataEntradaNaRede { get; set; }

    public bool Ativo { get; set; }

    public int QuantidadeDeUnidades { get; set; }

    public static FranqueadoDto DeModelo(Franqueado franqueado, int quantidadeDeUnidades = 0)
    {
        return new FranqueadoDto
        {
            Id = franqueado.Id,
            Nome = franqueado.Nome,
            Cpf = franqueado.Cpf,
            Email = franqueado.Email,
            Telefone = franqueado.Telefone,
            Cidade = franqueado.Cidade,
            Uf = franqueado.Uf,
            DataEntradaNaRede = franqueado.DataEntradaNaRede,
            Ativo = franqueado.Ativo,
            QuantidadeDeUnidades = quantidadeDeUnidades
        };
    }
}

/// <summary>
/// Filtros aceitos na listagem de franqueados.
/// </summary>
public sealed class ConsultaFranqueadosDto : ParametrosDeConsulta
{
    /// <summary>Texto pesquisado no nome, CPF ou e-mail.</summary>
    public string? Busca { get; set; }

    /// <summary>Filtra por franqueados ativos ou inativos.</summary>
    public bool? Ativo { get; set; }
}
