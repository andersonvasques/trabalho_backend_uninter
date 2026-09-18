using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados para apurar (gerar ou recalcular) os royalties de uma competência.
/// </summary>
public sealed class GerarRoyaltiesDto
{
    [Range(2000, 2100, ErrorMessage = "Informe um ano válido.")]
    public int Ano { get; set; }

    [Range(1, 12, ErrorMessage = "O mês deve estar entre 1 e 12.")]
    public int Mes { get; set; }

    /// <summary>
    /// Unidade específica. Quando não informada, a apuração
    /// é feita para todas as unidades ativas da rede.
    /// </summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>Dia do vencimento da cobrança no mês seguinte (padrão: 10).</summary>
    [Range(1, 28, ErrorMessage = "O dia de vencimento deve estar entre 1 e 28.")]
    public int DiaDoVencimento { get; set; } = 10;
}

/// <summary>
/// Dados para registrar o pagamento de uma cobrança de royalty.
/// </summary>
public sealed class RegistrarPagamentoRoyaltyDto
{
    [Range(0.01, 10000000, ErrorMessage = "O valor pago deve ser maior que zero.")]
    public decimal ValorPago { get; set; }

    /// <summary>Data do pagamento. Quando não informada, usa a data atual.</summary>
    public DateTime? DataPagamento { get; set; }
}

/// <summary>
/// Representação de uma cobrança de royalty devolvida pela API.
/// </summary>
public sealed class RoyaltyDto
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public string? UnidadeNome { get; set; }

    public int Ano { get; set; }

    public int Mes { get; set; }

    public string Competencia { get; set; } = string.Empty;

    public decimal FaturamentoBase { get; set; }

    public decimal PercentualAplicado { get; set; }

    public decimal ValorDevido { get; set; }

    public decimal ValorPago { get; set; }

    public decimal SaldoDevedor { get; set; }

    public SituacaoRoyalty Situacao { get; set; }

    public DateTime DataVencimento { get; set; }

    public DateTime? DataPagamento { get; set; }

    public DateTime GeradoEm { get; set; }

    public static RoyaltyDto DeModelo(Royalty royalty)
    {
        return new RoyaltyDto
        {
            Id = royalty.Id,
            UnidadeFranqueadaId = royalty.UnidadeFranqueadaId,
            UnidadeNome = royalty.Unidade?.NomeFantasia,
            Ano = royalty.Ano,
            Mes = royalty.Mes,
            Competencia = $"{royalty.Mes:00}/{royalty.Ano}",
            FaturamentoBase = royalty.FaturamentoBase,
            PercentualAplicado = royalty.PercentualAplicado,
            ValorDevido = royalty.ValorDevido,
            ValorPago = royalty.ValorPago,
            SaldoDevedor = royalty.ObterSaldoDevedor(),
            Situacao = royalty.Situacao,
            DataVencimento = royalty.DataVencimento,
            DataPagamento = royalty.DataPagamento,
            GeradoEm = royalty.GeradoEm
        };
    }
}

/// <summary>
/// Filtros aceitos na listagem de royalties.
/// </summary>
public sealed class ConsultaRoyaltiesDto : ParametrosDeConsulta
{
    public int? UnidadeFranqueadaId { get; set; }

    public int? Ano { get; set; }

    public int? Mes { get; set; }

    public SituacaoRoyalty? Situacao { get; set; }
}
