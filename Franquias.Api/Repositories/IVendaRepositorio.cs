using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados das vendas.
/// </summary>
public interface IVendaRepositorio : IRepositorioBase<Venda>
{
    /// <summary>Lista as vendas com filtros, ordenação e paginação.</summary>
    Task<ResultadoPaginado<Venda>> ListarPaginadoAsync(
        ConsultaVendasDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Obtém a venda com os itens, a unidade e o usuário carregados.</summary>
    Task<Venda?> ObterCompletaAsync(int id, CancellationToken cancellationToken);

    /// <summary>Gera o próximo número sequencial de venda.</summary>
    Task<string> GerarNumeroAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Soma o faturamento confirmado de uma unidade dentro do período informado.
    /// </summary>
    Task<decimal> CalcularFaturamentoAsync(
        int unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken);
}
