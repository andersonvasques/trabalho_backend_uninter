using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs;

/// <summary>
/// Período consultado pelos relatórios gerenciais.
/// Exemplo: GET /api/relatorios/faturamento?dataInicial=2026-01-01&amp;dataFinal=2026-03-31
/// </summary>
public sealed class ConsultaPeriodoDto
{
    [Required(ErrorMessage = "Informe a data inicial do período.")]
    public DateTime DataInicial { get; set; }

    [Required(ErrorMessage = "Informe a data final do período.")]
    public DateTime DataFinal { get; set; }

    /// <summary>Unidade específica. Quando não informada, considera toda a rede.</summary>
    public int? UnidadeFranqueadaId { get; set; }
}

/// <summary>
/// Faturamento consolidado de uma unidade no período.
/// </summary>
public sealed class FaturamentoPorUnidadeDto
{
    public int UnidadeFranqueadaId { get; set; }

    public string UnidadeNome { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public SituacaoUnidade Situacao { get; set; }

    public int QuantidadeDeVendas { get; set; }

    public decimal Faturamento { get; set; }

    public decimal TicketMedio { get; set; }
}

/// <summary>
/// Posição de uma unidade no ranking de faturamento.
/// </summary>
public sealed class RankingUnidadeDto
{
    public int Posicao { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public string UnidadeNome { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public int QuantidadeDeVendas { get; set; }

    public decimal Faturamento { get; set; }

    public decimal ParticipacaoPercentual { get; set; }
}

/// <summary>
/// Produto ou serviço com maior volume de vendas no período.
/// </summary>
public sealed class ProdutoMaisVendidoDto
{
    public int ProdutoServicoId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public int QuantidadeVendida { get; set; }

    public decimal ValorTotal { get; set; }
}

/// <summary>
/// Item de estoque em situação crítica (igual ou abaixo do mínimo).
/// </summary>
public sealed class EstoqueCriticoDto
{
    public int UnidadeFranqueadaId { get; set; }

    public string UnidadeNome { get; set; } = string.Empty;

    public int ProdutoServicoId { get; set; }

    public string ProdutoSku { get; set; } = string.Empty;

    public string ProdutoNome { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public int QuantidadeMinima { get; set; }

    public int QuantidadeParaRepor { get; set; }
}

/// <summary>
/// Total de royalties apurados para uma unidade no período.
/// </summary>
public sealed class RoyaltiesPorUnidadeDto
{
    public int UnidadeFranqueadaId { get; set; }

    public string UnidadeNome { get; set; } = string.Empty;

    public decimal FaturamentoBase { get; set; }

    public decimal TotalDevido { get; set; }

    public decimal TotalPago { get; set; }

    public decimal SaldoDevedor { get; set; }

    public int QuantidadeDeCobrancas { get; set; }
}

/// <summary>
/// Quantidade de chamados agrupada por situação.
/// </summary>
public sealed class ChamadosPorStatusDto
{
    public StatusChamado Status { get; set; }

    public int Quantidade { get; set; }
}

/// <summary>
/// Quantidade de chamados agrupada por prioridade.
/// </summary>
public sealed class ChamadosPorPrioridadeDto
{
    public PrioridadeChamado Prioridade { get; set; }

    public int Quantidade { get; set; }
}

/// <summary>
/// Painel com os principais indicadores da rede.
/// </summary>
public sealed class PainelGerencialDto
{
    public DateTime DataInicial { get; set; }

    public DateTime DataFinal { get; set; }

    public int TotalDeUnidades { get; set; }

    public int UnidadesAtivas { get; set; }

    public int UnidadesInativas { get; set; }

    public int QuantidadeDeVendas { get; set; }

    public decimal FaturamentoTotal { get; set; }

    public decimal TicketMedio { get; set; }

    public decimal RoyaltiesDevidos { get; set; }

    public decimal RoyaltiesPagos { get; set; }

    public int ItensComEstoqueCritico { get; set; }

    public int ChamadosEmAberto { get; set; }

    public IReadOnlyCollection<RankingUnidadeDto> TopUnidades { get; set; }
        = new List<RankingUnidadeDto>();

    public IReadOnlyCollection<ProdutoMaisVendidoDto> TopProdutos { get; set; }
        = new List<ProdutoMaisVendidoDto>();
}
