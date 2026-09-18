using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados específicas das unidades franqueadas.
/// </summary>
public interface IUnidadeRepositorio : IRepositorioBase<UnidadeFranqueada>
{
    /// <summary>Lista as unidades aplicando filtros, ordenação e paginação.</summary>
    Task<ResultadoPaginado<UnidadeFranqueada>> ListarPaginadoAsync(
        ConsultaUnidadesDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Obtém a unidade com a franqueadora e o franqueado carregados.</summary>
    Task<UnidadeFranqueada?> ObterCompletaAsync(int id, CancellationToken cancellationToken);

    /// <summary>Indica se o CNPJ já pertence a outra unidade.</summary>
    Task<bool> CnpjJaCadastradoAsync(string cnpj, int? idIgnorado, CancellationToken cancellationToken);

    /// <summary>Indica se o código interno já pertence a outra unidade.</summary>
    Task<bool> CodigoJaCadastradoAsync(string codigo, int? idIgnorado, CancellationToken cancellationToken);

    /// <summary>Lista as unidades ativas da rede.</summary>
    Task<IReadOnlyCollection<UnidadeFranqueada>> ListarAtivasAsync(CancellationToken cancellationToken);
}
