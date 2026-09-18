using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de fornecedores.
/// </summary>
public sealed class FornecedorRepositorio : RepositorioBase<Fornecedor>, IFornecedorRepositorio
{
    public FornecedorRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<Fornecedor>> ListarPaginadoAsync(
        ConsultaFornecedoresDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<Fornecedor> consulta = Conjunto
            .AsNoTracking()
            .Include(fornecedor => fornecedor.Produtos)
                .ThenInclude(vinculo => vinculo.ProdutoServico);

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim();

            consulta = consulta.Where(fornecedor =>
                fornecedor.RazaoSocial.Contains(termo) ||
                fornecedor.NomeFantasia.Contains(termo) ||
                fornecedor.Cnpj.Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Cidade))
        {
            string cidade = filtros.Cidade.Trim();
            consulta = consulta.Where(fornecedor => fornecedor.Cidade.Contains(cidade));
        }

        if (filtros.Ativo.HasValue)
        {
            consulta = consulta.Where(fornecedor => fornecedor.Ativo == filtros.Ativo.Value);
        }

        int total = await consulta.CountAsync(cancellationToken);

        consulta = filtros.Decrescente
            ? consulta.OrderByDescending(fornecedor => fornecedor.RazaoSocial)
            : consulta.OrderBy(fornecedor => fornecedor.RazaoSocial);

        List<Fornecedor> itens = await consulta
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Fornecedor>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<Fornecedor?> ObterComProdutosAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(fornecedor => fornecedor.Produtos)
                .ThenInclude(vinculo => vinculo.ProdutoServico)
            .FirstOrDefaultAsync(fornecedor => fornecedor.Id == id, cancellationToken);
    }

    public async Task<bool> CnpjJaCadastradoAsync(
        string cnpj,
        int? idIgnorado,
        CancellationToken cancellationToken)
    {
        return await Conjunto
            .AsNoTracking()
            .AnyAsync(
                fornecedor => fornecedor.Cnpj == cnpj &&
                              (idIgnorado == null || fornecedor.Id != idIgnorado),
                cancellationToken);
    }
}
