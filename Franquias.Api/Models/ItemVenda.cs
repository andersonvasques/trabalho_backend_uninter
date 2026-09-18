using Franquias.Api.Common;

namespace Franquias.Api.Models;

/// <summary>
/// Produto ou serviço incluído em uma venda, com quantidade e preço praticado.
/// </summary>
public sealed class ItemVenda
{
    /// <summary>Identificador único do item.</summary>
    public int Id { get; set; }

    /// <summary>Venda à qual o item pertence.</summary>
    public int VendaId { get; set; }

    /// <summary>Dados da venda.</summary>
    public Venda Venda { get; set; } = null!;

    /// <summary>Produto ou serviço vendido.</summary>
    public int ProdutoServicoId { get; set; }

    /// <summary>Dados do produto ou serviço.</summary>
    public ProdutoServico ProdutoServico { get; set; } = null!;

    /// <summary>Descrição do item no momento da venda (histórico).</summary>
    public string DescricaoItem { get; set; } = string.Empty;

    /// <summary>Quantidade vendida.</summary>
    public int Quantidade { get; set; }

    /// <summary>Preço unitário praticado na venda.</summary>
    public decimal PrecoUnitario { get; set; }

    /// <summary>Valor do item (quantidade x preço unitário).</summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Calcula o subtotal do item validando quantidade e preço.
    /// </summary>
    public void CalcularSubtotal()
    {
        if (Quantidade <= 0)
        {
            throw new RegraDeNegocioException("A quantidade de cada item deve ser maior que zero.");
        }

        if (PrecoUnitario < 0m)
        {
            throw new RegraDeNegocioException("O preço unitário não pode ser negativo.");
        }

        Subtotal = decimal.Round(PrecoUnitario * Quantidade, 2);
    }
}
