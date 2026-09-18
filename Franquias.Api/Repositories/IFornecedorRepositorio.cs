using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados dos fornecedores.
/// </summary>
public interface IFornecedorRepositorio : IRepositorioBase<Fornecedor>
{
    /// <summary>Lista os fornecedores com filtros, ordenação e paginação.</summary>
    Task<ResultadoPaginado<Fornecedor>> ListarPaginadoAsync(
        ConsultaFornecedoresDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Obtém o fornecedor com os produtos associados.</summary>
    Task<Fornecedor?> ObterComProdutosAsync(int id, CancellationToken cancellationToken);

    /// <summary>Indica se o CNPJ já pertence a outro fornecedor.</summary>
    Task<bool> CnpjJaCadastradoAsync(string cnpj, int? idIgnorado, CancellationToken cancellationToken);
}
