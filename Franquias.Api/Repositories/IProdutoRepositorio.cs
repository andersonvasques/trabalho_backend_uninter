using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados dos produtos e serviços do catálogo.
/// </summary>
public interface IProdutoRepositorio : IRepositorioBase<ProdutoServico>
{
    /// <summary>Lista os itens do catálogo com filtros, ordenação e paginação.</summary>
    Task<ResultadoPaginado<ProdutoServico>> ListarPaginadoAsync(
        ConsultaProdutosDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Obtém o item com a categoria carregada.</summary>
    Task<ProdutoServico?> ObterComCategoriaAsync(int id, CancellationToken cancellationToken);

    /// <summary>Indica se o SKU já pertence a outro item.</summary>
    Task<bool> SkuJaCadastradoAsync(string sku, int? idIgnorado, CancellationToken cancellationToken);
}
