using Franquias.Api.Common;

namespace Franquias.Api.Models;

/// <summary>
/// Cobrança de royalty de uma unidade referente a uma competência (mês/ano).
/// O valor é sempre calculado sobre o faturamento confirmado do período.
/// </summary>
public sealed class Royalty
{
    /// <summary>Identificador único da cobrança.</summary>
    public int Id { get; set; }

    /// <summary>Unidade cobrada.</summary>
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Dados da unidade.</summary>
    public UnidadeFranqueada Unidade { get; set; } = null!;

    /// <summary>Ano da competência.</summary>
    public int Ano { get; set; }

    /// <summary>Mês da competência (1 a 12).</summary>
    public int Mes { get; set; }

    /// <summary>Faturamento confirmado da unidade no período.</summary>
    public decimal FaturamentoBase { get; set; }

    /// <summary>Percentual aplicado sobre o faturamento.</summary>
    public decimal PercentualAplicado { get; set; }

    /// <summary>Valor devido pela unidade.</summary>
    public decimal ValorDevido { get; set; }

    /// <summary>Valor efetivamente pago.</summary>
    public decimal ValorPago { get; set; }

    /// <summary>Situação da cobrança.</summary>
    public SituacaoRoyalty Situacao { get; set; } = SituacaoRoyalty.Pendente;

    /// <summary>Data de vencimento da cobrança.</summary>
    public DateTime DataVencimento { get; set; }

    /// <summary>Data do pagamento, quando houver.</summary>
    public DateTime? DataPagamento { get; set; }

    /// <summary>Data em que a cobrança foi apurada.</summary>
    public DateTime GeradoEm { get; set; }

    /// <summary>Data da última alteração.</summary>
    public DateTime? AtualizadoEm { get; set; }

    /// <summary>
    /// Calcula o valor devido: faturamento x percentual.
    /// </summary>
    public void Calcular(decimal faturamento, decimal percentual)
    {
        if (faturamento < 0m)
        {
            throw new RegraDeNegocioException("O faturamento base não pode ser negativo.");
        }

        if (percentual < 0m || percentual > 100m)
        {
            throw new RegraDeNegocioException("O percentual de royalty deve estar entre 0 e 100.");
        }

        FaturamentoBase = decimal.Round(faturamento, 2);
        PercentualAplicado = percentual;
        ValorDevido = decimal.Round(faturamento * (percentual / 100m), 2);
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Registra o pagamento total ou parcial da cobrança.
    /// </summary>
    public void RegistrarPagamento(decimal valorPago, DateTime dataPagamento)
    {
        if (Situacao == SituacaoRoyalty.Cancelado)
        {
            throw new RegraDeNegocioException("Não é possível pagar uma cobrança cancelada.");
        }

        if (Situacao == SituacaoRoyalty.Pago)
        {
            throw new RegraDeNegocioException("Esta cobrança já está quitada.");
        }

        if (valorPago <= 0m)
        {
            throw new RegraDeNegocioException("O valor pago deve ser maior que zero.");
        }

        if (ValorDevido <= 0m)
        {
            throw new RegraDeNegocioException(
                "A competência não gerou valor a pagar, pois a unidade não teve faturamento no período.");
        }

        decimal saldoDevedor = ObterSaldoDevedor();

        if (valorPago > saldoDevedor)
        {
            throw new RegraDeNegocioException(
                $"O valor informado ({valorPago:0.00}) é maior que o saldo devedor ({saldoDevedor:0.00}).");
        }

        ValorPago = decimal.Round(ValorPago + valorPago, 2);
        DataPagamento = dataPagamento;
        AtualizadoEm = DateTime.UtcNow;

        Situacao = ValorPago >= ValorDevido
            ? SituacaoRoyalty.Pago
            : SituacaoRoyalty.Pendente;
    }

    /// <summary>
    /// Valor que ainda falta pagar.
    /// </summary>
    public decimal ObterSaldoDevedor()
    {
        decimal saldo = ValorDevido - ValorPago;
        return saldo < 0m ? 0m : decimal.Round(saldo, 2);
    }
}
