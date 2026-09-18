using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de cadastro da franqueadora e dos franqueados da rede.
/// </summary>
public interface IRedeService
{
    // ----- Franqueadora -----

    Task<IReadOnlyCollection<FranqueadoraDto>> ListarFranqueadorasAsync(
        CancellationToken cancellationToken);

    Task<FranqueadoraDto> ObterFranqueadoraAsync(int id, CancellationToken cancellationToken);

    Task<FranqueadoraDto> CriarFranqueadoraAsync(
        FranqueadoraEntradaDto dados,
        CancellationToken cancellationToken);

    Task<FranqueadoraDto> AtualizarFranqueadoraAsync(
        int id,
        FranqueadoraEntradaDto dados,
        CancellationToken cancellationToken);

    // ----- Franqueados -----

    Task<ResultadoPaginado<FranqueadoDto>> ListarFranqueadosAsync(
        ConsultaFranqueadosDto filtros,
        CancellationToken cancellationToken);

    Task<FranqueadoDto> ObterFranqueadoAsync(int id, CancellationToken cancellationToken);

    Task<FranqueadoDto> CriarFranqueadoAsync(
        FranqueadoEntradaDto dados,
        CancellationToken cancellationToken);

    Task<FranqueadoDto> AtualizarFranqueadoAsync(
        int id,
        FranqueadoEntradaDto dados,
        CancellationToken cancellationToken);

    /// <summary>Ativa ou inativa o franqueado (exclusão lógica).</summary>
    Task<FranqueadoDto> AlterarSituacaoFranqueadoAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken);
}
