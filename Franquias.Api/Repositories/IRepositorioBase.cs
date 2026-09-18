using System.Linq.Expressions;

namespace Franquias.Api.Repositories;

/// <summary>
/// Contrato genérico com as operações comuns de acesso a dados.
/// Evita repetir o mesmo código de CRUD em todos os repositórios.
/// </summary>
/// <typeparam name="TEntidade">Entidade mapeada pelo Entity Framework Core.</typeparam>
public interface IRepositorioBase<TEntidade> where TEntidade : class
{
    /// <summary>Consulta sem rastreamento, usada para montar filtros adicionais.</summary>
    IQueryable<TEntidade> Consultar();

    /// <summary>Obtém um registro pela chave primária.</summary>
    Task<TEntidade?> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Obtém o primeiro registro que atende ao filtro informado.</summary>
    Task<TEntidade?> ObterPrimeiroAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken);

    /// <summary>Lista todos os registros que atendem ao filtro informado.</summary>
    Task<IReadOnlyCollection<TEntidade>> ListarAsync(
        Expression<Func<TEntidade, bool>>? filtro,
        CancellationToken cancellationToken);

    /// <summary>Indica se existe algum registro que atende ao filtro.</summary>
    Task<bool> ExisteAsync(
        Expression<Func<TEntidade, bool>> filtro,
        CancellationToken cancellationToken);

    /// <summary>Adiciona um novo registro ao contexto.</summary>
    Task AdicionarAsync(TEntidade entidade, CancellationToken cancellationToken);

    /// <summary>Marca um registro como alterado.</summary>
    void Atualizar(TEntidade entidade);

    /// <summary>Marca um registro para remoção.</summary>
    void Remover(TEntidade entidade);

    /// <summary>Confirma no banco de dados todas as alterações pendentes.</summary>
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken);
}
