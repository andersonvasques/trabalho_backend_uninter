using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados dos chamados de suporte.
/// </summary>
public interface IChamadoRepositorio : IRepositorioBase<ChamadoSuporte>
{
    /// <summary>Lista os chamados com filtros, ordenação e paginação.</summary>
    Task<ResultadoPaginado<ChamadoSuporte>> ListarPaginadoAsync(
        ConsultaChamadosDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Obtém o chamado com as interações e a unidade carregadas.</summary>
    Task<ChamadoSuporte?> ObterCompletoAsync(int id, CancellationToken cancellationToken);

    /// <summary>Gera o próximo protocolo de atendimento.</summary>
    Task<string> GerarProtocoloAsync(CancellationToken cancellationToken);

    /// <summary>Adiciona uma interação ao chamado.</summary>
    Task AdicionarInteracaoAsync(ChamadoInteracao interacao, CancellationToken cancellationToken);

    /// <summary>Conta os chamados por situação, opcionalmente filtrando por unidade.</summary>
    Task<IReadOnlyCollection<ChamadoSuporte>> ListarParaIndicadoresAsync(
        int? unidadeId,
        CancellationToken cancellationToken);
}
