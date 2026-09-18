using System.Linq.Expressions;
using Franquias.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação genérica do repositório utilizando o Entity Framework Core.
/// Todas as operações de acesso a dados são assíncronas (async/await).
/// </summary>
public class RepositorioBase<TEntidade> : IRepositorioBase<TEntidade>
    where TEntidade : class
{
    /// <summary>Contexto do banco de dados recebido por injeção de dependência.</summary>
    protected readonly AppDbContext Contexto;

    /// <summary>Conjunto de dados da entidade.</summary>
    protected readonly DbSet<TEntidade> Conjunto;

    public RepositorioBase(AppDbContext contexto)
    {
        Contexto = contexto;
        Conjunto = contexto.Set<TEntidade>();
    }

    public IQueryable<TEntidade> Consultar()
    {
        // AsNoTracking: os objetos serão usados apenas para leitura,
        // o que deixa a consulta mais rápida.
        return Conjunto.AsNoTracking();
    }

    public virtual async Task<TEntidade?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<TEntidade?> ObterPrimeiroAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken)
    {
        return await Conjunto.FirstOrDefaultAsync(filtro, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TEntidade>> ListarAsync(
        Expression<Func<TEntidade, bool>>? filtro,
        CancellationToken cancellationToken)
    {
        IQueryable<TEntidade> consulta = Conjunto.AsNoTracking();

        if (filtro is not null)
        {
            consulta = consulta.Where(filtro);
        }

        return await consulta.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken)
    {
        return await Conjunto.AsNoTracking().AnyAsync(filtro, cancellationToken);
    }

    public async Task AdicionarAsync(TEntidade entidade, CancellationToken cancellationToken)
    {
        await Conjunto.AddAsync(entidade, cancellationToken);
    }

    public void Atualizar(TEntidade entidade)
    {
        Conjunto.Update(entidade);
    }

    public void Remover(TEntidade entidade)
    {
        Conjunto.Remove(entidade);
    }

    public async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken)
    {
        return await Contexto.SaveChangesAsync(cancellationToken);
    }
}
