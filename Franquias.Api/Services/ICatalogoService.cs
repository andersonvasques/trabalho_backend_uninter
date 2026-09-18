using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras das categorias do catálogo padronizado da rede.
/// </summary>
public interface ICategoriaService
{
    Task<IReadOnlyCollection<CategoriaDto>> ListarAsync(
        bool? apenasAtivas,
        CancellationToken cancellationToken);

    Task<CategoriaDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<CategoriaDto> CriarAsync(CategoriaEntradaDto dados, CancellationToken cancellationToken);

    Task<CategoriaDto> AtualizarAsync(
        int id,
        CategoriaEntradaDto dados,
        CancellationToken cancellationToken);

    Task<CategoriaDto> AlterarSituacaoAsync(int id, bool ativa, CancellationToken cancellationToken);
}

/// <summary>
/// Regras dos produtos e serviços comercializados pela rede.
/// </summary>
public interface IProdutoService
{
    Task<ResultadoPaginado<ProdutoDto>> ListarAsync(
        ConsultaProdutosDto filtros,
        CancellationToken cancellationToken);

    Task<ProdutoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<ProdutoDto> CriarAsync(CriarProdutoDto dados, CancellationToken cancellationToken);

    Task<ProdutoDto> AtualizarAsync(
        int id,
        AtualizarProdutoDto dados,
        CancellationToken cancellationToken);

    /// <summary>Ativa ou inativa o item (exclusão lógica).</summary>
    Task<ProdutoDto> AlterarSituacaoAsync(int id, bool ativo, CancellationToken cancellationToken);
}

/// <summary>
/// Regras dos fornecedores homologados pela franqueadora.
/// </summary>
public interface IFornecedorService
{
    Task<ResultadoPaginado<FornecedorDto>> ListarAsync(
        ConsultaFornecedoresDto filtros,
        CancellationToken cancellationToken);

    Task<FornecedorDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<FornecedorDto> CriarAsync(FornecedorEntradaDto dados, CancellationToken cancellationToken);

    Task<FornecedorDto> AtualizarAsync(
        int id,
        FornecedorEntradaDto dados,
        CancellationToken cancellationToken);

    Task<FornecedorDto> AlterarSituacaoAsync(int id, bool ativo, CancellationToken cancellationToken);

    /// <summary>Associa um produto ao fornecedor.</summary>
    Task<FornecedorDto> VincularProdutoAsync(
        int fornecedorId,
        VincularProdutoDto dados,
        CancellationToken cancellationToken);

    /// <summary>Remove a associação entre o fornecedor e um produto.</summary>
    Task<FornecedorDto> DesvincularProdutoAsync(
        int fornecedorId,
        int produtoId,
        CancellationToken cancellationToken);
}
