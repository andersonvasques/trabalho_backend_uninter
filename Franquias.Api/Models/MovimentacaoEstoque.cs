namespace Franquias.Api.Models;

/// <summary>
/// Histórico de cada entrada, saída ou ajuste realizado no estoque
/// de uma unidade. Mantido para rastreabilidade das operações.
/// </summary>
public sealed class MovimentacaoEstoque
{
    /// <summary>Identificador único da movimentação.</summary>
    public int Id { get; set; }

    /// <summary>Saldo de estoque movimentado.</summary>
    public int EstoqueUnidadeId { get; set; }

    /// <summary>Dados do saldo de estoque.</summary>
    public EstoqueUnidade EstoqueUnidade { get; set; } = null!;

    /// <summary>Tipo da movimentação.</summary>
    public TipoMovimentacaoEstoque Tipo { get; set; }

    /// <summary>Quantidade movimentada.</summary>
    public int Quantidade { get; set; }

    /// <summary>Saldo antes da movimentação.</summary>
    public int SaldoAnterior { get; set; }

    /// <summary>Saldo depois da movimentação.</summary>
    public int SaldoAtual { get; set; }

    /// <summary>Motivo informado pelo usuário ou gerado pelo sistema.</summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>Venda que originou a movimentação, quando houver.</summary>
    public int? VendaId { get; set; }

    /// <summary>Dados da venda.</summary>
    public Venda? Venda { get; set; }

    /// <summary>Usuário responsável pela movimentação.</summary>
    public int? UsuarioId { get; set; }

    /// <summary>Dados do usuário.</summary>
    public Usuario? Usuario { get; set; }

    /// <summary>Data e hora da movimentação.</summary>
    public DateTime OcorridaEm { get; set; }
}
