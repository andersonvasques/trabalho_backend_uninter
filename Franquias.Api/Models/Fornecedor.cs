namespace Franquias.Api.Models;

/// <summary>
/// Empresa homologada pela franqueadora para abastecer as unidades.
/// </summary>
public sealed class Fornecedor
{
    /// <summary>Identificador único do fornecedor.</summary>
    public int Id { get; set; }

    /// <summary>Razão social do fornecedor.</summary>
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome fantasia do fornecedor.</summary>
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>CNPJ do fornecedor (somente dígitos). Não pode se repetir.</summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>E-mail comercial.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Telefone comercial.</summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>Cidade do fornecedor.</summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>Unidade federativa do fornecedor.</summary>
    public string Uf { get; set; } = string.Empty;

    /// <summary>Indica se o fornecedor continua homologado.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Data de criação do cadastro.</summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>Data da última alteração do cadastro.</summary>
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Itens fornecidos por esta empresa.</summary>
    public ICollection<ProdutoFornecedor> Produtos { get; set; } = new List<ProdutoFornecedor>();
}
