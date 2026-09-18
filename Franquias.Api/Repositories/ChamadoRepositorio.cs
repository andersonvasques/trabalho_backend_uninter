using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de chamados de suporte.
/// </summary>
public sealed class ChamadoRepositorio : RepositorioBase<ChamadoSuporte>, IChamadoRepositorio
{
    public ChamadoRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<ChamadoSuporte>> ListarPaginadoAsync(
        ConsultaChamadosDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<ChamadoSuporte> consulta = Conjunto
            .AsNoTracking()
            .Include(chamado => chamado.Unidade)
            .Include(chamado => chamado.UsuarioAbertura);

        if (filtros.UnidadeFranqueadaId.HasValue)
        {
            consulta = consulta.Where(chamado =>
                chamado.UnidadeFranqueadaId == filtros.UnidadeFranqueadaId.Value);
        }

        if (filtros.Status.HasValue)
        {
            consulta = consulta.Where(chamado => chamado.Status == filtros.Status.Value);
        }

        if (filtros.Prioridade.HasValue)
        {
            consulta = consulta.Where(chamado => chamado.Prioridade == filtros.Prioridade.Value);
        }

        if (filtros.Categoria.HasValue)
        {
            consulta = consulta.Where(chamado => chamado.Categoria == filtros.Categoria.Value);
        }

        if (filtros.ApenasEmAberto)
        {
            consulta = consulta.Where(chamado =>
                chamado.Status == StatusChamado.Aberto ||
                chamado.Status == StatusChamado.EmAtendimento);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim();

            consulta = consulta.Where(chamado =>
                chamado.Protocolo.Contains(termo) ||
                chamado.Titulo.Contains(termo) ||
                chamado.Descricao.Contains(termo));
        }

        int total = await consulta.CountAsync(cancellationToken);

        string campo = filtros.OrdenarPor?.Trim().ToLower() ?? string.Empty;

        consulta = campo switch
        {
            "prioridade" => filtros.Decrescente
                ? consulta.OrderByDescending(chamado => chamado.Prioridade)
                : consulta.OrderBy(chamado => chamado.Prioridade),

            "status" => filtros.Decrescente
                ? consulta.OrderByDescending(chamado => chamado.Status)
                : consulta.OrderBy(chamado => chamado.Status),

            _ => filtros.Decrescente
                ? consulta.OrderByDescending(chamado => chamado.AbertoEm)
                : consulta.OrderBy(chamado => chamado.AbertoEm)
        };

        List<ChamadoSuporte> itens = await consulta
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<ChamadoSuporte>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<ChamadoSuporte?> ObterCompletoAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(chamado => chamado.Unidade)
            .Include(chamado => chamado.UsuarioAbertura)
            .Include(chamado => chamado.Interacoes)
                .ThenInclude(interacao => interacao.Usuario)
            .FirstOrDefaultAsync(chamado => chamado.Id == id, cancellationToken);
    }

    public async Task<string> GerarProtocoloAsync(CancellationToken cancellationToken)
    {
        int ano = DateTime.UtcNow.Year;

        int quantidade = await Conjunto
            .AsNoTracking()
            .CountAsync(chamado => chamado.AbertoEm.Year == ano, cancellationToken);

        return $"CH-{ano}-{(quantidade + 1):D6}";
    }

    public async Task AdicionarInteracaoAsync(
        ChamadoInteracao interacao,
        CancellationToken cancellationToken)
    {
        await Contexto.ChamadosInteracoes.AddAsync(interacao, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChamadoSuporte>> ListarParaIndicadoresAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        IQueryable<ChamadoSuporte> consulta = Conjunto.AsNoTracking();

        if (unidadeId.HasValue)
        {
            consulta = consulta.Where(chamado => chamado.UnidadeFranqueadaId == unidadeId.Value);
        }

        return await consulta.ToListAsync(cancellationToken);
    }
}
