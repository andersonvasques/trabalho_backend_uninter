using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Linha simplificada de venda usada pelos relatórios.
/// </summary>
public sealed record VendaConsolidada(
    int UnidadeFranqueadaId,
    string UnidadeNome,
    string Cidade,
    SituacaoUnidade Situacao,
    decimal ValorTotal);

/// <summary>
/// Linha simplificada de item vendido usada pelos relatórios.
/// </summary>
public sealed record ItemVendidoConsolidado(
    int ProdutoServicoId,
    string Sku,
    string Nome,
    string Categoria,
    int Quantidade,
    decimal Subtotal);

/// <summary>
/// Consultas utilizadas pelos relatórios e indicadores gerenciais.
/// </summary>
public interface IRelatorioRepositorio
{
    /// <summary>Vendas confirmadas do período, já com os dados da unidade.</summary>
    Task<IReadOnlyCollection<VendaConsolidada>> ListarVendasConfirmadasAsync(
        int? unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken);

    /// <summary>Itens vendidos no período, já com os dados do produto.</summary>
    Task<IReadOnlyCollection<ItemVendidoConsolidado>> ListarItensVendidosAsync(
        int? unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken);

    /// <summary>Quantidade de unidades agrupadas por situação.</summary>
    Task<IReadOnlyCollection<UnidadeFranqueada>> ListarUnidadesAsync(
        int? unidadeId,
        CancellationToken cancellationToken);
}
