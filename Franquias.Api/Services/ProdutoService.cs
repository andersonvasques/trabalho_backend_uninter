using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras dos produtos e serviços do catálogo.
/// </summary>
public sealed class ProdutoService : IProdutoService
{
    private readonly IProdutoRepositorio _produtoRepositorio;
    private readonly IRepositorioBase<Categoria> _categoriaRepositorio;

    public ProdutoService(
        IProdutoRepositorio produtoRepositorio,
        IRepositorioBase<Categoria> categoriaRepositorio)
    {
        _produtoRepositorio = produtoRepositorio;
        _categoriaRepositorio = categoriaRepositorio;
    }

    public async Task<ResultadoPaginado<ProdutoDto>> ListarAsync(
        ConsultaProdutosDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<ProdutoServico> pagina =
            await _produtoRepositorio.ListarPaginadoAsync(filtros, cancellationToken);

        var itens = new List<ProdutoDto>();

        foreach (ProdutoServico produto in pagina.Itens)
        {
            itens.Add(ProdutoDto.DeModelo(produto));
        }

        return new ResultadoPaginado<ProdutoDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<ProdutoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        ProdutoServico produto = await ObterProdutoAsync(id, cancellationToken);

        return ProdutoDto.DeModelo(produto);
    }

    public async Task<ProdutoDto> CriarAsync(
        CriarProdutoDto dados,
        CancellationToken cancellationToken)
    {
        string sku = dados.Sku.Trim().ToUpper();

        bool skuEmUso = await _produtoRepositorio.SkuJaCadastradoAsync(sku, null, cancellationToken);

        if (skuEmUso)
        {
            throw new ConflitoException($"Já existe um item cadastrado com o SKU {sku}.");
        }

        await GarantirCategoriaValidaAsync(dados.CategoriaId, cancellationToken);

        var produto = new ProdutoServico
        {
            Sku = sku,
            Nome = dados.Nome.Trim(),
            Descricao = dados.Descricao.Trim(),
            CategoriaId = dados.CategoriaId,
            Tipo = dados.Tipo,
            PrecoBase = decimal.Round(dados.PrecoBase, 2),
            EstoqueMinimoPadrao = dados.Tipo == TipoItem.Servico ? 0 : dados.EstoqueMinimoPadrao,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        await _produtoRepositorio.AdicionarAsync(produto, cancellationToken);
        await _produtoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        ProdutoServico? criado = await _produtoRepositorio.ObterComCategoriaAsync(
            produto.Id,
            cancellationToken);

        return ProdutoDto.DeModelo(criado ?? produto);
    }

    public async Task<ProdutoDto> AtualizarAsync(
        int id,
        AtualizarProdutoDto dados,
        CancellationToken cancellationToken)
    {
        ProdutoServico produto = await ObterProdutoAsync(id, cancellationToken);

        await GarantirCategoriaValidaAsync(dados.CategoriaId, cancellationToken);

        produto.Nome = dados.Nome.Trim();
        produto.Descricao = dados.Descricao.Trim();
        produto.CategoriaId = dados.CategoriaId;
        produto.Tipo = dados.Tipo;
        produto.PrecoBase = decimal.Round(dados.PrecoBase, 2);
        produto.EstoqueMinimoPadrao = dados.Tipo == TipoItem.Servico ? 0 : dados.EstoqueMinimoPadrao;
        produto.AtualizadoEm = DateTime.UtcNow;

        await _produtoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return ProdutoDto.DeModelo(produto);
    }

    public async Task<ProdutoDto> AlterarSituacaoAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken)
    {
        ProdutoServico produto = await ObterProdutoAsync(id, cancellationToken);

        // Exclusão lógica: o item deixa de ser vendido, mas o histórico é mantido.
        produto.Ativo = ativo;
        produto.AtualizadoEm = DateTime.UtcNow;

        await _produtoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return ProdutoDto.DeModelo(produto);
    }

    private async Task<ProdutoServico> ObterProdutoAsync(int id, CancellationToken cancellationToken)
    {
        ProdutoServico? produto = await _produtoRepositorio.ObterComCategoriaAsync(id, cancellationToken);

        if (produto is null)
        {
            throw new NaoEncontradoException($"O produto ou serviço {id} não foi encontrado.");
        }

        return produto;
    }

    private async Task GarantirCategoriaValidaAsync(int categoriaId, CancellationToken cancellationToken)
    {
        Categoria? categoria = await _categoriaRepositorio.ObterPorIdAsync(categoriaId, cancellationToken);

        if (categoria is null)
        {
            throw new NaoEncontradoException($"A categoria {categoriaId} não foi encontrada.");
        }

        if (!categoria.Ativa)
        {
            throw new RegraDeNegocioException(
                $"A categoria '{categoria.Nome}' está inativa e não pode receber novos itens.");
        }
    }
}
