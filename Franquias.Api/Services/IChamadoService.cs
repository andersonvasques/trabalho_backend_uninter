using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras dos chamados de suporte entre unidades e franqueadora.
/// </summary>
public interface IChamadoService
{
    Task<ResultadoPaginado<ChamadoDto>> ListarAsync(
        ConsultaChamadosDto filtros,
        CancellationToken cancellationToken);

    Task<ChamadoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<ChamadoDto> AbrirAsync(AbrirChamadoDto dados, CancellationToken cancellationToken);

    /// <summary>Registra uma nova mensagem e, opcionalmente, altera o status.</summary>
    Task<ChamadoDto> RegistrarInteracaoAsync(
        int id,
        RegistrarInteracaoDto dados,
        CancellationToken cancellationToken);

    /// <summary>Encerra o chamado registrando a solução aplicada.</summary>
    Task<ChamadoDto> EncerrarAsync(
        int id,
        EncerrarChamadoDto dados,
        CancellationToken cancellationToken);
}
