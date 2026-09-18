namespace Franquias.Api.Models;

/// <summary>
/// Produto ou serviço padronizado pela franqueadora e
/// comercializado por todas as unidades da rede.
/// </summary>
public sealed class ProdutoServico
{
    /// <summary>Identificador único do item.</summary>
    public int Id { get; set; }

    /// <summary>Código interno do item (SKU). Não pode se repetir.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>Nome comercial do item.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Descrição detalhada do item.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Categoria à qual o item pertence.</summary>
    public int CategoriaId { get; set; }

    /// <summary>Dados da categoria.</summary>
    public Categoria Categoria { get; set; } = null!;

    /// <summary>Indica se o item é um produto físico ou um serviço.</summary>
    public TipoItem Tipo { get; set; } = TipoItem.Produto;

    /// <summary>Preço sugerido pela rede.</summary>
    public decimal PrecoBase { get; set; }

    /// <summary>Quantidade mínima sugerida de estoque nas unidades.</summary>
    public int EstoqueMinimoPadrao { get; set; }

    /// <summary>Indica se o item continua disponível para venda.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Data de criação do cadastro.</summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>Data da última alteração do cadastro.</summary>
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>Fornecedores homologados para o item.</summary>
    public ICollection<ProdutoFornecedor> Fornecedores { get; set; } = new List<ProdutoFornecedor>();

    /// <summary>Saldos de estoque do item nas unidades.</summary>
    public ICollection<EstoqueUnidade> Estoques { get; set; } = new List<EstoqueUnidade>();

    /// <summary>Itens de venda que referenciam este produto/serviço.</summary>
    public ICollection<ItemVenda> ItensVendidos { get; set; } = new List<ItemVenda>();

    /// <summary>
    /// Regra de negócio: apenas produtos físicos controlam estoque.
    /// </summary>
    public bool ControlaEstoque()
    {
        return Tipo == TipoItem.Produto;
    }
}
