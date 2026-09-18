namespace Franquias.Api.Models;

/// <summary>
/// Associação entre um item do catálogo e um fornecedor homologado.
/// Representa o relacionamento muitos-para-muitos entre
/// <see cref="ProdutoServico"/> e <see cref="Fornecedor"/>, com dados adicionais.
/// </summary>
public sealed class ProdutoFornecedor
{
    /// <summary>Item fornecido.</summary>
    public int ProdutoServicoId { get; set; }

    /// <summary>Dados do item.</summary>
    public ProdutoServico ProdutoServico { get; set; } = null!;

    /// <summary>Fornecedor do item.</summary>
    public int FornecedorId { get; set; }

    /// <summary>Dados do fornecedor.</summary>
    public Fornecedor Fornecedor { get; set; } = null!;

    /// <summary>Preço de custo negociado.</summary>
    public decimal PrecoCusto { get; set; }

    /// <summary>Prazo médio de entrega em dias.</summary>
    public int PrazoEntregaDias { get; set; }

    /// <summary>Indica se este é o fornecedor preferencial do item.</summary>
    public bool Preferencial { get; set; }

    /// <summary>Data em que a associação foi criada.</summary>
    public DateTime CriadaEm { get; set; }
}
