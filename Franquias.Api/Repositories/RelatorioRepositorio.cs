using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Consultas de leitura utilizadas pelos relatórios gerenciais.
///
/// As linhas são trazidas do banco já filtradas e projetadas em objetos
/// simples; os agrupamentos e somas são concluídos na camada de serviço,
/// garantindo o mesmo resultado em qualquer provedor de banco de dados.
/// </summary>
public sealed class RelatorioRepositorio : IRelatorioRepositorio
{
    private readonly AppDbContext _contexto;

    public RelatorioRepositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IReadOnlyCollection<VendaConsolidada>> ListarVendasConfirmadasAsync(
        int? unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken)
    {
        IQueryable<Venda> consulta = _contexto.Vendas
            .AsNoTracking()
            .Where(venda =>
                venda.Status == StatusVenda.Confirmada &&
                venda.DataVenda >= dataInicial &&
                venda.DataVenda <= dataFinal);

        if (unidadeId.HasValue)
        {
            consulta = consulta.Where(venda => venda.UnidadeFranqueadaId == unidadeId.Value);
        }

        return await consulta
            .Select(venda => new VendaConsolidada(
                venda.UnidadeFranqueadaId,
                venda.Unidade.NomeFantasia,
                venda.Unidade.Cidade,
                venda.Unidade.Situacao,
                venda.ValorTotal))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ItemVendidoConsolidado>> ListarItensVendidosAsync(
        int? unidadeId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken cancellationToken)
    {
        IQueryable<ItemVenda> consulta = _contexto.ItensVenda
            .AsNoTracking()
            .Where(item =>
                item.Venda.Status == StatusVenda.Confirmada &&
                item.Venda.DataVenda >= dataInicial &&
                item.Venda.DataVenda <= dataFinal);

        if (unidadeId.HasValue)
        {
            consulta = consulta.Where(item => item.Venda.UnidadeFranqueadaId == unidadeId.Value);
        }

        return await consulta
            .Select(item => new ItemVendidoConsolidado(
                item.ProdutoServicoId,
                item.ProdutoServico.Sku,
                item.ProdutoServico.Nome,
                item.ProdutoServico.Categoria.Nome,
                item.Quantidade,
                item.Subtotal))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UnidadeFranqueada>> ListarUnidadesAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        IQueryable<UnidadeFranqueada> consulta = _contexto.Unidades.AsNoTracking();

        if (unidadeId.HasValue)
        {
            consulta = consulta.Where(unidade => unidade.Id == unidadeId.Value);
        }

        return await consulta.ToListAsync(cancellationToken);
    }
}
