using Franquias.Api.Common;

namespace Franquias.Api.Models;

/// <summary>
/// Venda realizada por uma unidade franqueada.
/// O valor total é sempre calculado a partir dos itens.
/// </summary>
public sealed class Venda
{
    /// <summary>Identificador único da venda.</summary>
    public int Id { get; set; }

    /// <summary>Número sequencial da venda dentro da rede (ex.: "VD-2026-000001").</summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>Unidade que registrou a venda.</summary>
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Dados da unidade.</summary>
    public UnidadeFranqueada Unidade { get; set; } = null!;

    /// <summary>Usuário que registrou a venda.</summary>
    public int UsuarioId { get; set; }

    /// <summary>Dados do usuário.</summary>
    public Usuario Usuario { get; set; } = null!;

    /// <summary>Data em que a venda ocorreu.</summary>
    public DateTime DataVenda { get; set; }

    /// <summary>Situação da venda.</summary>
    public StatusVenda Status { get; set; } = StatusVenda.Confirmada;

    /// <summary>Forma de pagamento utilizada.</summary>
    public FormaPagamento FormaPagamento { get; set; } = FormaPagamento.Dinheiro;

    /// <summary>Desconto aplicado sobre a soma dos itens.</summary>
    public decimal Desconto { get; set; }

    /// <summary>Valor total da venda (soma dos itens menos o desconto).</summary>
    public decimal ValorTotal { get; set; }

    /// <summary>Nome do cliente, quando informado.</summary>
    public string? Cliente { get; set; }

    /// <summary>Observações gerais da venda.</summary>
    public string? Observacao { get; set; }

    /// <summary>Data de criação do registro.</summary>
    public DateTime CriadaEm { get; set; }

    /// <summary>Data do cancelamento, quando houver.</summary>
    public DateTime? CanceladaEm { get; set; }

    /// <summary>Motivo do cancelamento, quando houver.</summary>
    public string? MotivoCancelamento { get; set; }

    /// <summary>Itens que compõem a venda.</summary>
    public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();

    /// <summary>
    /// Recalcula o valor total a partir dos itens e do desconto.
    /// Regra de negócio: toda venda precisa de pelo menos um item.
    /// </summary>
    public void RecalcularTotal()
    {
        if (Itens.Count == 0)
        {
            throw new RegraDeNegocioException("A venda deve possuir pelo menos um item.");
        }

        decimal somaDosItens = 0m;

        foreach (ItemVenda item in Itens)
        {
            item.CalcularSubtotal();
            somaDosItens += item.Subtotal;
        }

        if (Desconto < 0m)
        {
            throw new RegraDeNegocioException("O desconto não pode ser negativo.");
        }

        if (Desconto > somaDosItens)
        {
            throw new RegraDeNegocioException("O desconto não pode ser maior que o valor dos itens.");
        }

        ValorTotal = decimal.Round(somaDosItens - Desconto, 2);
    }

    /// <summary>
    /// Cancela a venda mantendo o histórico (exclusão lógica).
    /// </summary>
    public void Cancelar(string motivo)
    {
        if (Status == StatusVenda.Cancelada)
        {
            throw new RegraDeNegocioException($"A venda {Numero} já está cancelada.");
        }

        Status = StatusVenda.Cancelada;
        CanceladaEm = DateTime.UtcNow;
        MotivoCancelamento = motivo;
    }
}
