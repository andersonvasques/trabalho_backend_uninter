using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de apuração e cobrança dos royalties das unidades.
/// </summary>
public interface IRoyaltyService
{
    Task<ResultadoPaginado<RoyaltyDto>> ListarAsync(
        ConsultaRoyaltiesDto filtros,
        CancellationToken cancellationToken);

    Task<RoyaltyDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Apura os royalties de uma competência a partir do faturamento
    /// confirmado de cada unidade no período.
    /// </summary>
    Task<IReadOnlyCollection<RoyaltyDto>> ApurarAsync(
        GerarRoyaltiesDto dados,
        CancellationToken cancellationToken);

    /// <summary>Registra o pagamento (total ou parcial) de uma cobrança.</summary>
    Task<RoyaltyDto> RegistrarPagamentoAsync(
        int id,
        RegistrarPagamentoRoyaltyDto dados,
        CancellationToken cancellationToken);

    /// <summary>Cancela uma cobrança mantendo o histórico.</summary>
    Task<RoyaltyDto> CancelarAsync(int id, CancellationToken cancellationToken);
}
