using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de royalties.
/// </summary>
public sealed class RoyaltyRepositorio : RepositorioBase<Royalty>, IRoyaltyRepositorio
{
    public RoyaltyRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<Royalty>> ListarPaginadoAsync(
        ConsultaRoyaltiesDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<Royalty> consulta = Conjunto
            .AsNoTracking()
            .Include(royalty => royalty.Unidade);

        if (filtros.UnidadeFranqueadaId.HasValue)
        {
            consulta = consulta.Where(royalty =>
                royalty.UnidadeFranqueadaId == filtros.UnidadeFranqueadaId.Value);
        }

        if (filtros.Ano.HasValue)
        {
            consulta = consulta.Where(royalty => royalty.Ano == filtros.Ano.Value);
        }

        if (filtros.Mes.HasValue)
        {
            consulta = consulta.Where(royalty => royalty.Mes == filtros.Mes.Value);
        }

        if (filtros.Situacao.HasValue)
        {
            consulta = consulta.Where(royalty => royalty.Situacao == filtros.Situacao.Value);
        }

        int total = await consulta.CountAsync(cancellationToken);

        List<Royalty> itens = await consulta
            .OrderByDescending(royalty => royalty.Ano)
            .ThenByDescending(royalty => royalty.Mes)
            .ThenBy(royalty => royalty.UnidadeFranqueadaId)
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Royalty>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<Royalty?> ObterPorCompetenciaAsync(
        int unidadeId,
        int ano,
        int mes,
        CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(royalty => royalty.Unidade)
            .FirstOrDefaultAsync(
                royalty => royalty.UnidadeFranqueadaId == unidadeId &&
                           royalty.Ano == ano &&
                           royalty.Mes == mes,
                cancellationToken);
    }

    public async Task<Royalty?> ObterComUnidadeAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(royalty => royalty.Unidade)
            .FirstOrDefaultAsync(royalty => royalty.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Royalty>> ListarPorPeriodoAsync(
        int? unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken)
    {
        int competenciaInicial = (dataInicial.Year * 100) + dataInicial.Month;
        int competenciaFinal = (dataFinal.Year * 100) + dataFinal.Month;

        IQueryable<Royalty> consulta = Conjunto
            .AsNoTracking()
            .Include(royalty => royalty.Unidade)
            .Where(royalty =>
                ((royalty.Ano * 100) + royalty.Mes) >= competenciaInicial &&
                ((royalty.Ano * 100) + royalty.Mes) <= competenciaFinal);

        if (unidadeId.HasValue)
        {
            consulta = consulta.Where(royalty => royalty.UnidadeFranqueadaId == unidadeId.Value);
        }

        return await consulta
            .OrderBy(royalty => royalty.UnidadeFranqueadaId)
            .ThenBy(royalty => royalty.Ano)
            .ThenBy(royalty => royalty.Mes)
            .ToListAsync(cancellationToken);
    }
}
