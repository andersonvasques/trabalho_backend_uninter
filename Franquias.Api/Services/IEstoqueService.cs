using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de controle do estoque das unidades franqueadas.
/// </summary>
public interface IEstoqueService
{
    Task<ResultadoPaginado<EstoqueDto>> ListarAsync(
        ConsultaEstoqueDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Consulta o saldo de um produto em uma unidade.</summary>
    Task<EstoqueDto> ObterSaldoAsync(
        int unidadeId,
        int produtoId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Registra uma entrada, saída ou ajuste de estoque.
    /// O saldo nunca pode ficar negativo.
    /// </summary>
    Task<EstoqueDto> MovimentarAsync(
        MovimentarEstoqueDto dados,
        CancellationToken cancellationToken);

    /// <summary>Define a quantidade mínima desejada de um produto na unidade.</summary>
    Task<EstoqueDto> DefinirEstoqueMinimoAsync(
        DefinirEstoqueMinimoDto dados,
        CancellationToken cancellationToken);

    /// <summary>Lista os itens com saldo igual ou abaixo do mínimo.</summary>
    Task<IReadOnlyCollection<EstoqueDto>> ListarAbaixoDoMinimoAsync(
        int? unidadeId,
        CancellationToken cancellationToken);

    /// <summary>Consulta o histórico de movimentações.</summary>
    Task<ResultadoPaginado<MovimentacaoEstoqueDto>> ListarMovimentacoesAsync(
        ConsultaMovimentacoesDto filtros,
        CancellationToken cancellationToken);
}
