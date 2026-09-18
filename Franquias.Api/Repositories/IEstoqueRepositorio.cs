using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados do estoque das unidades.
/// </summary>
public interface IEstoqueRepositorio : IRepositorioBase<EstoqueUnidade>
{
    /// <summary>Lista os saldos de estoque com filtros e paginação.</summary>
    Task<ResultadoPaginado<EstoqueUnidade>> ListarPaginadoAsync(
        ConsultaEstoqueDto filtros,
        CancellationToken cancellationToken);

    /// <summary>
    /// Obtém o saldo de um produto em uma unidade com rastreamento
    /// habilitado, permitindo alterar a quantidade.
    /// </summary>
    Task<EstoqueUnidade?> ObterParaAtualizacaoAsync(
        int unidadeId,
        int produtoId,
        CancellationToken cancellationToken);

    /// <summary>Lista os itens com saldo igual ou abaixo do mínimo.</summary>
    Task<IReadOnlyCollection<EstoqueUnidade>> ListarAbaixoDoMinimoAsync(
        int? unidadeId,
        CancellationToken cancellationToken);

    /// <summary>Lista o histórico de movimentações com filtros e paginação.</summary>
    Task<ResultadoPaginado<MovimentacaoEstoque>> ListarMovimentacoesAsync(
        ConsultaMovimentacoesDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Registra uma movimentação no histórico.</summary>
    Task AdicionarMovimentacaoAsync(
        MovimentacaoEstoque movimentacao,
        CancellationToken cancellationToken);
}
