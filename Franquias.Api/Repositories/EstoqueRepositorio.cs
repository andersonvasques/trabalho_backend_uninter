using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de estoque.
/// </summary>
public sealed class EstoqueRepositorio : RepositorioBase<EstoqueUnidade>, IEstoqueRepositorio
{
    public EstoqueRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<EstoqueUnidade>> ListarPaginadoAsync(
        ConsultaEstoqueDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<EstoqueUnidade> consulta = Conjunto
            .AsNoTracking()
            .Include(estoque => estoque.Unidade)
            .Include(estoque => estoque.ProdutoServico);

        if (filtros.UnidadeFranqueadaId.HasValue)
        {
            consulta = consulta.Where(estoque =>
                estoque.UnidadeFranqueadaId == filtros.UnidadeFranqueadaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim();

            consulta = consulta.Where(estoque =>
                estoque.ProdutoServico.Nome.Contains(termo) ||
                estoque.ProdutoServico.Sku.Contains(termo));
        }

        if (filtros.ApenasAbaixoDoMinimo)
        {
            consulta = consulta.Where(estoque => estoque.Quantidade <= estoque.QuantidadeMinima);
        }

        int total = await consulta.CountAsync(cancellationToken);

        string campo = filtros.OrdenarPor?.Trim().ToLower() ?? string.Empty;

        consulta = campo switch
        {
            "quantidade" => filtros.Decrescente
                ? consulta.OrderByDescending(estoque => estoque.Quantidade)
                : consulta.OrderBy(estoque => estoque.Quantidade),

            "unidade" => filtros.Decrescente
                ? consulta.OrderByDescending(estoque => estoque.Unidade.NomeFantasia)
                : consulta.OrderBy(estoque => estoque.Unidade.NomeFantasia),

            _ => filtros.Decrescente
                ? consulta.OrderByDescending(estoque => estoque.ProdutoServico.Nome)
                : consulta.OrderBy(estoque => estoque.ProdutoServico.Nome)
        };

        List<EstoqueUnidade> itens = await consulta
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<EstoqueUnidade>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<EstoqueUnidade?> ObterParaAtualizacaoAsync(
        int unidadeId,
        int produtoId,
        CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(estoque => estoque.ProdutoServico)
            .FirstOrDefaultAsync(
                estoque => estoque.UnidadeFranqueadaId == unidadeId &&
                           estoque.ProdutoServicoId == produtoId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<EstoqueUnidade>> ListarAbaixoDoMinimoAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        IQueryable<EstoqueUnidade> consulta = Conjunto
            .AsNoTracking()
            .Include(estoque => estoque.Unidade)
            .Include(estoque => estoque.ProdutoServico)
            .Where(estoque => estoque.Quantidade <= estoque.QuantidadeMinima);

        if (unidadeId.HasValue)
        {
            consulta = consulta.Where(estoque => estoque.UnidadeFranqueadaId == unidadeId.Value);
        }

        return await consulta
            .OrderBy(estoque => estoque.Quantidade)
            .ToListAsync(cancellationToken);
    }

    public async Task<ResultadoPaginado<MovimentacaoEstoque>> ListarMovimentacoesAsync(
        ConsultaMovimentacoesDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<MovimentacaoEstoque> consulta = Contexto.MovimentacoesEstoque
            .AsNoTracking()
            .Include(movimentacao => movimentacao.EstoqueUnidade)
                .ThenInclude(estoque => estoque.ProdutoServico);

        if (filtros.UnidadeFranqueadaId.HasValue)
        {
            consulta = consulta.Where(movimentacao =>
                movimentacao.EstoqueUnidade.UnidadeFranqueadaId == filtros.UnidadeFranqueadaId.Value);
        }

        if (filtros.ProdutoServicoId.HasValue)
        {
            consulta = consulta.Where(movimentacao =>
                movimentacao.EstoqueUnidade.ProdutoServicoId == filtros.ProdutoServicoId.Value);
        }

        if (filtros.Tipo.HasValue)
        {
            consulta = consulta.Where(movimentacao => movimentacao.Tipo == filtros.Tipo.Value);
        }

        if (filtros.DataInicial.HasValue)
        {
            DateTime inicio = filtros.DataInicial.Value.Date;
            consulta = consulta.Where(movimentacao => movimentacao.OcorridaEm >= inicio);
        }

        if (filtros.DataFinal.HasValue)
        {
            DateTime fim = filtros.DataFinal.Value.Date.AddDays(1).AddTicks(-1);
            consulta = consulta.Where(movimentacao => movimentacao.OcorridaEm <= fim);
        }

        int total = await consulta.CountAsync(cancellationToken);

        List<MovimentacaoEstoque> itens = await consulta
            .OrderByDescending(movimentacao => movimentacao.OcorridaEm)
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<MovimentacaoEstoque>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task AdicionarMovimentacaoAsync(
        MovimentacaoEstoque movimentacao,
        CancellationToken cancellationToken)
    {
        await Contexto.MovimentacoesEstoque.AddAsync(movimentacao, cancellationToken);
    }
}
