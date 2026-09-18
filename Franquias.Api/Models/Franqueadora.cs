namespace Franquias.Api.Models;

/// <summary>
/// Empresa dona da marca (matriz da rede de franquias).
/// É ela quem define o percentual padrão de royalty cobrado das unidades.
/// </summary>
public sealed class Franqueadora
{
    /// <summary>Identificador único da franqueadora.</summary>
    public int Id { get; set; }

    /// <summary>Razão social registrada.</summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome fantasia / marca da rede.</summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>CNPJ da franqueadora (somente dígitos). Não pode se repetir.</summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>E-mail de contato da matriz.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Telefone de contato da matriz.</summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>Cidade da sede.</summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>Unidade federativa da sede.</summary>
    public string Uf { get; set; } = string.Empty;

    /// <summary>Percentual de royalty sugerido para novas unidades (ex.: 5,00 = 5%).</summary>
    public decimal PercentualRoyaltyPadrao { get; set; }

    /// <summary>Data de fundação da rede.</summary>
    public DateTime DataFundacao { get; set; }

    /// <summary>Indica se a rede está ativa.</summary>
    public bool Ativa { get; set; } = true;

    /// <summary>Data de criação do cadastro.</summary>
    public DateTime CriadaEm { get; set; }

    /// <summary>Data da última alteração do cadastro.</summary>
    public DateTime? AtualizadaEm { get; set; }

    /// <summary>Unidades franqueadas pertencentes à rede.</summary>
    public ICollection<UnidadeFranqueada> Unidades { get; set; } = new List<UnidadeFranqueada>();
}
