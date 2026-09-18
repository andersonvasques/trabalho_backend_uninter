using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de vendas.
/// </summary>
public sealed class VendaRepositorio : RepositorioBase<Venda>, IVendaRepositorio
{
    public VendaRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<Venda>> ListarPaginadoAsync(
        ConsultaVendasDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<Venda> consulta = Conjunto
            .AsNoTracking()
            .Include(venda => venda.Unidade)
            .Include(venda => venda.Usuario)
            .Include(venda => venda.Itens);

        if (filtros.UnidadeFranqueadaId.HasValue)
        {
            consulta = consulta.Where(venda =>
                venda.UnidadeFranqueadaId == filtros.UnidadeFranqueadaId.Value);
        }

        if (filtros.DataInicial.HasValue)
        {
            DateTime inicio = filtros.DataInicial.Value.Date;
            consulta = consulta.Where(venda => venda.DataVenda >= inicio);
        }

        if (filtros.DataFinal.HasValue)
        {
            DateTime fim = filtros.DataFinal.Value.Date.AddDays(1).AddTicks(-1);
            consulta = consulta.Where(venda => venda.DataVenda <= fim);
        }

        if (filtros.Status.HasValue)
        {
            consulta = consulta.Where(venda => venda.Status == filtros.Status.Value);
        }

        if (filtros.FormaPagamento.HasValue)
        {
            consulta = consulta.Where(venda => venda.FormaPagamento == filtros.FormaPagamento.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim();

            consulta = consulta.Where(venda =>
                venda.Numero.Contains(termo) ||
                (venda.Cliente != null && venda.Cliente.Contains(termo)));
        }

        int total = await consulta.CountAsync(cancellationToken);

        string campo = filtros.OrdenarPor?.Trim().ToLower() ?? string.Empty;

        consulta = campo switch
        {
            "valor" => filtros.Decrescente
                ? consulta.OrderByDescending(venda => venda.ValorTotal)
                : consulta.OrderBy(venda => venda.ValorTotal),

            "numero" => filtros.Decrescente
                ? consulta.OrderByDescending(venda => venda.Numero)
                : consulta.OrderBy(venda => venda.Numero),

            _ => filtros.Decrescente
                ? consulta.OrderByDescending(venda => venda.DataVenda)
                : consulta.OrderBy(venda => venda.DataVenda)
        };

        List<Venda> itens = await consulta
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Venda>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<Venda?> ObterCompletaAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(venda => venda.Unidade)
            .Include(venda => venda.Usuario)
            .Include(venda => venda.Itens)
                .ThenInclude(item => item.ProdutoServico)
            .FirstOrDefaultAsync(venda => venda.Id == id, cancellationToken);
    }

    public async Task<string> GerarNumeroAsync(CancellationToken cancellationToken)
    {
        int ano = DateTime.UtcNow.Year;

        int quantidade = await Conjunto
            .AsNoTracking()
            .CountAsync(venda => venda.DataVenda.Year == ano, cancellationToken);

        return $"VD-{ano}-{(quantidade + 1):D6}";
    }

    public async Task<decimal> CalcularFaturamentoAsync(
        int unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken)
    {
        // Somente vendas confirmadas entram no faturamento.
        List<decimal> valores = await Conjunto
            .AsNoTracking()
            .Where(venda =>
                venda.UnidadeFranqueadaId == unidadeId &&
                venda.Status == StatusVenda.Confirmada &&
                venda.DataVenda >= dataInicial &&
                venda.DataVenda <= dataFinal)
            .Select(venda => venda.ValorTotal)
            .ToListAsync(cancellationToken);

        decimal total = 0m;

        foreach (decimal valor in valores)
        {
            total += valor;
        }

        return decimal.Round(total, 2);
    }
}
