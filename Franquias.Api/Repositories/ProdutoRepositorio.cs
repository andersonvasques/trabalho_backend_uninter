using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de produtos e serviços.
/// </summary>
public sealed class ProdutoRepositorio : RepositorioBase<ProdutoServico>, IProdutoRepositorio
{
    public ProdutoRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<ProdutoServico>> ListarPaginadoAsync(
        ConsultaProdutosDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<ProdutoServico> consulta = Conjunto
            .AsNoTracking()
            .Include(produto => produto.Categoria);

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim();

            consulta = consulta.Where(produto =>
                produto.Nome.Contains(termo) ||
                produto.Sku.Contains(termo) ||
                produto.Descricao.Contains(termo));
        }

        if (filtros.CategoriaId.HasValue)
        {
            consulta = consulta.Where(produto => produto.CategoriaId == filtros.CategoriaId.Value);
        }

        if (filtros.Tipo.HasValue)
        {
            consulta = consulta.Where(produto => produto.Tipo == filtros.Tipo.Value);
        }

        if (filtros.Ativo.HasValue)
        {
            consulta = consulta.Where(produto => produto.Ativo == filtros.Ativo.Value);
        }

        int total = await consulta.CountAsync(cancellationToken);

        consulta = OrdenarConsulta(consulta, filtros);

        List<ProdutoServico> itens = await consulta
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<ProdutoServico>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<ProdutoServico?> ObterComCategoriaAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(produto => produto.Categoria)
            .FirstOrDefaultAsync(produto => produto.Id == id, cancellationToken);
    }

    public async Task<bool> SkuJaCadastradoAsync(
        string sku,
        int? idIgnorado,
        CancellationToken cancellationToken)
    {
        string skuNormalizado = sku.Trim().ToUpper();

        return await Conjunto
            .AsNoTracking()
            .AnyAsync(
                produto => produto.Sku.ToUpper() == skuNormalizado &&
                           (idIgnorado == null || produto.Id != idIgnorado),
                cancellationToken);
    }

    private static IQueryable<ProdutoServico> OrdenarConsulta(
        IQueryable<ProdutoServico> consulta,
        ConsultaProdutosDto filtros)
    {
        string campo = filtros.OrdenarPor?.Trim().ToLower() ?? string.Empty;

        return campo switch
        {
            "sku" => filtros.Decrescente
                ? consulta.OrderByDescending(produto => produto.Sku)
                : consulta.OrderBy(produto => produto.Sku),

            "preco" => filtros.Decrescente
                ? consulta.OrderByDescending(produto => produto.PrecoBase)
                : consulta.OrderBy(produto => produto.PrecoBase),

            "categoria" => filtros.Decrescente
                ? consulta.OrderByDescending(produto => produto.Categoria.Nome)
                : consulta.OrderBy(produto => produto.Categoria.Nome),

            _ => filtros.Decrescente
                ? consulta.OrderByDescending(produto => produto.Nome)
                : consulta.OrderBy(produto => produto.Nome)
        };
    }
}
