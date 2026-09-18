using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados das cobranças de royalties.
/// </summary>
public interface IRoyaltyRepositorio : IRepositorioBase<Royalty>
{
    /// <summary>Lista as cobranças com filtros e paginação.</summary>
    Task<ResultadoPaginado<Royalty>> ListarPaginadoAsync(
        ConsultaRoyaltiesDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Obtém a cobrança de uma unidade em uma competência.</summary>
    Task<Royalty?> ObterPorCompetenciaAsync(
        int unidadeId,
        int ano,
        int mes,
        CancellationToken cancellationToken);

    /// <summary>Obtém a cobrança com a unidade carregada.</summary>
    Task<Royalty?> ObterComUnidadeAsync(int id, CancellationToken cancellationToken);

    /// <summary>Lista as cobranças de um período para os relatórios.</summary>
    Task<IReadOnlyCollection<Royalty>> ListarPorPeriodoAsync(
        int? unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken);
}
