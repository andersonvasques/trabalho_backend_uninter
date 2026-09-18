namespace Franquias.Api.Models;

/// <summary>
/// Pessoa que investe na marca e responde por uma ou mais unidades franqueadas.
/// </summary>
public sealed class Franqueado
{
    /// <summary>Identificador único do franqueado.</summary>
    public int Id { get; set; }

    /// <summary>Nome completo do franqueado.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>CPF do franqueado (somente dígitos). Não pode se repetir.</summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Telefone de contato.</summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>Cidade de residência.</summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>Unidade federativa de residência.</summary>
    public string Uf { get; set; } = string.Empty;

    /// <summary>Data de entrada do franqueado na rede.</summary>
    public DateTime DataEntradaNaRede { get; set; }

    /// <summary>Indica se o franqueado está ativo na rede.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Data de criação do cadastro.</summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>Data da última alteração do cadastro.</summary>
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Unidades sob responsabilidade do franqueado.</summary>
    public ICollection<UnidadeFranqueada> Unidades { get; set; } = new List<UnidadeFranqueada>();
}
