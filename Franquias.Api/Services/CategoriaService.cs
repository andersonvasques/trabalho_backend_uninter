using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras das categorias do catálogo.
/// </summary>
public sealed class CategoriaService : ICategoriaService
{
    private readonly IRepositorioBase<Categoria> _categoriaRepositorio;
    private readonly IProdutoRepositorio _produtoRepositorio;

    public CategoriaService(
        IRepositorioBase<Categoria> categoriaRepositorio,
        IProdutoRepositorio produtoRepositorio)
    {
        _categoriaRepositorio = categoriaRepositorio;
        _produtoRepositorio = produtoRepositorio;
    }

    public async Task<IReadOnlyCollection<CategoriaDto>> ListarAsync(
        bool? apenasAtivas,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Categoria> categorias = apenasAtivas.HasValue
            ? await _categoriaRepositorio.ListarAsync(
                categoria => categoria.Ativa == apenasAtivas.Value,
                cancellationToken)
            : await _categoriaRepositorio.ListarAsync(null, cancellationToken);

        IReadOnlyCollection<ProdutoServico> produtos =
            await _produtoRepositorio.ListarAsync(null, cancellationToken);

        var resultado = new List<CategoriaDto>();

        foreach (Categoria categoria in categorias.OrderBy(categoria => categoria.Nome))
        {
            int quantidade = produtos.Count(produto => produto.CategoriaId == categoria.Id);
            resultado.Add(CategoriaDto.DeModelo(categoria, quantidade));
        }

        return resultado;
    }

    public async Task<CategoriaDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        Categoria categoria = await ObterCategoriaAsync(id, cancellationToken);

        IReadOnlyCollection<ProdutoServico> produtos = await _produtoRepositorio.ListarAsync(
            produto => produto.CategoriaId == id,
            cancellationToken);

        return CategoriaDto.DeModelo(categoria, produtos.Count);
    }

    public async Task<CategoriaDto> CriarAsync(
        CategoriaEntradaDto dados,
        CancellationToken cancellationToken)
    {
        string nome = dados.Nome.Trim();

        bool nomeEmUso = await _categoriaRepositorio.ExisteAsync(
            categoria => categoria.Nome.ToLower() == nome.ToLower(),
            cancellationToken);

        if (nomeEmUso)
        {
            throw new ConflitoException($"Já existe uma categoria chamada '{nome}'.");
        }

        var categoria = new Categoria
        {
            Nome = nome,
            Descricao = dados.Descricao.Trim(),
            Ativa = true,
            CriadaEm = DateTime.UtcNow
        };

        await _categoriaRepositorio.AdicionarAsync(categoria, cancellationToken);
        await _categoriaRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return CategoriaDto.DeModelo(categoria);
    }

    public async Task<CategoriaDto> AtualizarAsync(
        int id,
        CategoriaEntradaDto dados,
        CancellationToken cancellationToken)
    {
        Categoria categoria = await ObterCategoriaAsync(id, cancellationToken);

        string nome = dados.Nome.Trim();

        bool nomeEmUso = await _categoriaRepositorio.ExisteAsync(
            outra => outra.Nome.ToLower() == nome.ToLower() && outra.Id != id,
            cancellationToken);

        if (nomeEmUso)
        {
            throw new ConflitoException($"Já existe outra categoria chamada '{nome}'.");
        }

        categoria.Nome = nome;
        categoria.Descricao = dados.Descricao.Trim();
        categoria.AtualizadaEm = DateTime.UtcNow;

        await _categoriaRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return CategoriaDto.DeModelo(categoria);
    }

    public async Task<CategoriaDto> AlterarSituacaoAsync(
        int id,
        bool ativa,
        CancellationToken cancellationToken)
    {
        Categoria categoria = await ObterCategoriaAsync(id, cancellationToken);

        if (!ativa)
        {
            IReadOnlyCollection<ProdutoServico> itensAtivos = await _produtoRepositorio.ListarAsync(
                produto => produto.CategoriaId == id && produto.Ativo,
                cancellationToken);

            if (itensAtivos.Count > 0)
            {
                throw new RegraDeNegocioException(
                    "Não é possível inativar uma categoria que ainda possui itens ativos.");
            }
        }

        categoria.Ativa = ativa;
        categoria.AtualizadaEm = DateTime.UtcNow;

        await _categoriaRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return CategoriaDto.DeModelo(categoria);
    }

    private async Task<Categoria> ObterCategoriaAsync(int id, CancellationToken cancellationToken)
    {
        Categoria? categoria = await _categoriaRepositorio.ObterPorIdAsync(id, cancellationToken);

        if (categoria is null)
        {
            throw new NaoEncontradoException($"A categoria {id} não foi encontrada.");
        }

        return categoria;
    }
}
