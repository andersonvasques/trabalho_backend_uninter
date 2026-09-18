using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;
using Franquias.Api.Validations;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados necessários para cadastrar uma unidade franqueada.
/// </summary>
public sealed class CriarUnidadeDto
{
    [Required(ErrorMessage = "O código da unidade é obrigatório.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "O código deve ter entre 2 e 20 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Informe a franqueadora da unidade.")]
    public int FranqueadoraId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe o franqueado responsável pela unidade.")]
    public int FranqueadoId { get; set; }

    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "A razão social deve ter entre 3 e 150 caracteres.")]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome fantasia deve ter entre 2 e 150 caracteres.")]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [Cnpj]
    public string Cnpj { get; set; } = string.Empty;

    [EmailOpcional]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "O logradouro deve ter no máximo 150 caracteres.")]
    public string Logradouro { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres.")]
    public string Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "A UF é obrigatória.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter 2 caracteres.")]
    public string Uf { get; set; } = string.Empty;

    [CepOpcional]
    public string Cep { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "O nome do responsável deve ter no máximo 150 caracteres.")]
    public string ResponsavelNome { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone do responsável deve ter no máximo 20 caracteres.")]
    public string ResponsavelTelefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de início do contrato é obrigatória.")]
    public DateTime DataInicioContrato { get; set; }

    [Range(0, 100, ErrorMessage = "O percentual de royalty deve estar entre 0 e 100.")]
    public decimal PercentualRoyalty { get; set; }

    [EnumDataType(typeof(SituacaoUnidade), ErrorMessage = "Situação inválida.")]
    public SituacaoUnidade Situacao { get; set; } = SituacaoUnidade.EmImplantacao;
}

/// <summary>
/// Dados alteráveis de uma unidade franqueada.
/// O CNPJ e a franqueadora não podem ser alterados após o cadastro.
/// </summary>
public sealed class AtualizarUnidadeDto
{
    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "A razão social deve ter entre 3 e 150 caracteres.")]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome fantasia deve ter entre 2 e 150 caracteres.")]
    public string NomeFantasia { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Informe o franqueado responsável pela unidade.")]
    public int FranqueadoId { get; set; }

    [EmailOpcional]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "O logradouro deve ter no máximo 150 caracteres.")]
    public string Logradouro { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres.")]
    public string Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "A UF é obrigatória.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter 2 caracteres.")]
    public string Uf { get; set; } = string.Empty;

    [CepOpcional]
    public string Cep { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "O nome do responsável deve ter no máximo 150 caracteres.")]
    public string ResponsavelNome { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone do responsável deve ter no máximo 20 caracteres.")]
    public string ResponsavelTelefone { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "O percentual de royalty deve estar entre 0 e 100.")]
    public decimal PercentualRoyalty { get; set; }

    [EnumDataType(typeof(SituacaoUnidade), ErrorMessage = "Situação inválida.")]
    public SituacaoUnidade Situacao { get; set; }
}

/// <summary>
/// Representação de uma unidade devolvida pela API.
/// </summary>
public sealed class UnidadeDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string RazaoSocial { get; set; } = string.Empty;

    public string NomeFantasia { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string EnderecoCompleto { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Uf { get; set; } = string.Empty;

    public string ResponsavelNome { get; set; } = string.Empty;

    public string ResponsavelTelefone { get; set; } = string.Empty;

    public DateTime DataInicioContrato { get; set; }

    public DateTime? DataEncerramentoContrato { get; set; }

    public decimal PercentualRoyalty { get; set; }

    public SituacaoUnidade Situacao { get; set; }

    public int FranqueadoraId { get; set; }

    public string? FranqueadoraNome { get; set; }

    public int FranqueadoId { get; set; }

    public string? FranqueadoNome { get; set; }

    public DateTime CriadaEm { get; set; }

    public static UnidadeDto DeModelo(UnidadeFranqueada unidade)
    {
        return new UnidadeDto
        {
            Id = unidade.Id,
            Codigo = unidade.Codigo,
            RazaoSocial = unidade.RazaoSocial,
            NomeFantasia = unidade.NomeFantasia,
            Cnpj = unidade.Cnpj,
            Email = unidade.Email,
            Telefone = unidade.Telefone,
            EnderecoCompleto = $"{unidade.Logradouro}, {unidade.Numero} - {unidade.Bairro}".Trim(),
            Cidade = unidade.Cidade,
            Uf = unidade.Uf,
            ResponsavelNome = unidade.ResponsavelNome,
            ResponsavelTelefone = unidade.ResponsavelTelefone,
            DataInicioContrato = unidade.DataInicioContrato,
            DataEncerramentoContrato = unidade.DataEncerramentoContrato,
            PercentualRoyalty = unidade.PercentualRoyalty,
            Situacao = unidade.Situacao,
            FranqueadoraId = unidade.FranqueadoraId,
            FranqueadoraNome = unidade.Franqueadora?.NomeFantasia,
            FranqueadoId = unidade.FranqueadoId,
            FranqueadoNome = unidade.Franqueado?.Nome,
            CriadaEm = unidade.CriadaEm
        };
    }
}

/// <summary>
/// Filtros aceitos na listagem de unidades.
/// Exemplo: GET /api/unidades?cidade=Curitiba&amp;situacao=Ativa&amp;pagina=1
/// </summary>
public sealed class ConsultaUnidadesDto : ParametrosDeConsulta
{
    /// <summary>Texto pesquisado no nome fantasia, razão social, CNPJ ou responsável.</summary>
    public string? Busca { get; set; }

    /// <summary>Filtra pela cidade da unidade.</summary>
    public string? Cidade { get; set; }

    /// <summary>Filtra pela UF da unidade.</summary>
    public string? Uf { get; set; }

    /// <summary>Filtra pela situação contratual.</summary>
    public SituacaoUnidade? Situacao { get; set; }

    /// <summary>Filtra pelo franqueado responsável.</summary>
    public int? FranqueadoId { get; set; }

    /// <summary>
    /// Filtra uma unidade específica. Usuários de unidade sempre
    /// têm esse filtro preenchido automaticamente com a própria unidade.
    /// </summary>
    public int? UnidadeFranqueadaId { get; set; }
}
