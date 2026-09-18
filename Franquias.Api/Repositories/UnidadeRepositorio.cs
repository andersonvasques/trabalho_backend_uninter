using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de unidades franqueadas.
/// Concentra as consultas com filtros, ordenação e paginação.
/// </summary>
public sealed class UnidadeRepositorio : RepositorioBase<UnidadeFranqueada>, IUnidadeRepositorio
{
    public UnidadeRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<UnidadeFranqueada>> ListarPaginadoAsync(
        ConsultaUnidadesDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<UnidadeFranqueada> consulta = Conjunto
            .AsNoTracking()
            .Include(unidade => unidade.Franqueadora)
            .Include(unidade => unidade.Franqueado);

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim();

            consulta = consulta.Where(unidade =>
                unidade.NomeFantasia.Contains(termo) ||
                unidade.RazaoSocial.Contains(termo) ||
                unidade.Cnpj.Contains(termo) ||
                unidade.Codigo.Contains(termo) ||
                unidade.ResponsavelNome.Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Cidade))
        {
            string cidade = filtros.Cidade.Trim();
            consulta = consulta.Where(unidade => unidade.Cidade.Contains(cidade));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Uf))
        {
            string uf = filtros.Uf.Trim().ToUpper();
            consulta = consulta.Where(unidade => unidade.Uf.ToUpper() == uf);
        }

        if (filtros.Situacao.HasValue)
        {
            consulta = consulta.Where(unidade => unidade.Situacao == filtros.Situacao.Value);
        }

        if (filtros.FranqueadoId.HasValue)
        {
            consulta = consulta.Where(unidade => unidade.FranqueadoId == filtros.FranqueadoId.Value);
        }

        if (filtros.UnidadeFranqueadaId.HasValue)
        {
            consulta = consulta.Where(unidade => unidade.Id == filtros.UnidadeFranqueadaId.Value);
        }

        int total = await consulta.CountAsync(cancellationToken);

        consulta = OrdenarConsulta(consulta, filtros);

        List<UnidadeFranqueada> itens = await consulta
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<UnidadeFranqueada>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<UnidadeFranqueada?> ObterCompletaAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(unidade => unidade.Franqueadora)
            .Include(unidade => unidade.Franqueado)
            .FirstOrDefaultAsync(unidade => unidade.Id == id, cancellationToken);
    }

    public async Task<bool> CnpjJaCadastradoAsync(
        string cnpj,
        int? idIgnorado,
        CancellationToken cancellationToken)
    {
        return await Conjunto
            .AsNoTracking()
            .AnyAsync(
                unidade => unidade.Cnpj == cnpj &&
                           (idIgnorado == null || unidade.Id != idIgnorado),
                cancellationToken);
    }

    public async Task<bool> CodigoJaCadastradoAsync(
        string codigo,
        int? idIgnorado,
        CancellationToken cancellationToken)
    {
        string codigoNormalizado = codigo.Trim().ToUpper();

        return await Conjunto
            .AsNoTracking()
            .AnyAsync(
                unidade => unidade.Codigo.ToUpper() == codigoNormalizado &&
                           (idIgnorado == null || unidade.Id != idIgnorado),
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<UnidadeFranqueada>> ListarAtivasAsync(
        CancellationToken cancellationToken)
    {
        return await Conjunto
            .AsNoTracking()
            .Where(unidade => unidade.Situacao == SituacaoUnidade.Ativa)
            .OrderBy(unidade => unidade.NomeFantasia)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<UnidadeFranqueada> OrdenarConsulta(
        IQueryable<UnidadeFranqueada> consulta,
        ConsultaUnidadesDto filtros)
    {
        string campo = filtros.OrdenarPor?.Trim().ToLower() ?? string.Empty;

        return campo switch
        {
            "codigo" => filtros.Decrescente
                ? consulta.OrderByDescending(unidade => unidade.Codigo)
                : consulta.OrderBy(unidade => unidade.Codigo),

            "cidade" => filtros.Decrescente
                ? consulta.OrderByDescending(unidade => unidade.Cidade)
                : consulta.OrderBy(unidade => unidade.Cidade),

            "situacao" => filtros.Decrescente
                ? consulta.OrderByDescending(unidade => unidade.Situacao)
                : consulta.OrderBy(unidade => unidade.Situacao),

            "datainiciocontrato" => filtros.Decrescente
                ? consulta.OrderByDescending(unidade => unidade.DataInicioContrato)
                : consulta.OrderBy(unidade => unidade.DataInicioContrato),

            _ => filtros.Decrescente
                ? consulta.OrderByDescending(unidade => unidade.NomeFantasia)
                : consulta.OrderBy(unidade => unidade.NomeFantasia)
        };
    }
}
