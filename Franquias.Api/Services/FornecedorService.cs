using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Validations;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras dos fornecedores e da associação com os produtos.
/// </summary>
public sealed class FornecedorService : IFornecedorService
{
    private readonly IFornecedorRepositorio _fornecedorRepositorio;
    private readonly IProdutoRepositorio _produtoRepositorio;
    private readonly IRepositorioBase<ProdutoFornecedor> _vinculoRepositorio;

    public FornecedorService(
        IFornecedorRepositorio fornecedorRepositorio,
        IProdutoRepositorio produtoRepositorio,
        IRepositorioBase<ProdutoFornecedor> vinculoRepositorio)
    {
        _fornecedorRepositorio = fornecedorRepositorio;
        _produtoRepositorio = produtoRepositorio;
        _vinculoRepositorio = vinculoRepositorio;
    }

    public async Task<ResultadoPaginado<FornecedorDto>> ListarAsync(
        ConsultaFornecedoresDto filtros,
        CancellationToken cancellationToken)
    {
        ResultadoPaginado<Fornecedor> pagina =
            await _fornecedorRepositorio.ListarPaginadoAsync(filtros, cancellationToken);

        var itens = new List<FornecedorDto>();

        foreach (Fornecedor fornecedor in pagina.Itens)
        {
            itens.Add(FornecedorDto.DeModelo(fornecedor));
        }

        return new ResultadoPaginado<FornecedorDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<FornecedorDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        Fornecedor fornecedor = await ObterFornecedorAsync(id, cancellationToken);

        return FornecedorDto.DeModelo(fornecedor);
    }

    public async Task<FornecedorDto> CriarAsync(
        FornecedorEntradaDto dados,
        CancellationToken cancellationToken)
    {
        string cnpj = ValidadorDeDocumentos.SomenteDigitos(dados.Cnpj);

        bool cnpjEmUso = await _fornecedorRepositorio.CnpjJaCadastradoAsync(cnpj, null, cancellationToken);

        if (cnpjEmUso)
        {
            throw new ConflitoException($"Já existe um fornecedor cadastrado com o CNPJ {cnpj}.");
        }

        var fornecedor = new Fornecedor
        {
            RazaoSocial = dados.RazaoSocial.Trim(),
            NomeFantasia = dados.NomeFantasia.Trim(),
            Cnpj = cnpj,
            Email = dados.Email.Trim().ToLower(),
            Telefone = dados.Telefone.Trim(),
            Cidade = dados.Cidade.Trim(),
            Uf = dados.Uf.Trim().ToUpper(),
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        await _fornecedorRepositorio.AdicionarAsync(fornecedor, cancellationToken);
        await _fornecedorRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorDto.DeModelo(fornecedor);
    }

    public async Task<FornecedorDto> AtualizarAsync(
        int id,
        FornecedorEntradaDto dados,
        CancellationToken cancellationToken)
    {
        Fornecedor fornecedor = await ObterFornecedorAsync(id, cancellationToken);

        string cnpj = ValidadorDeDocumentos.SomenteDigitos(dados.Cnpj);

        bool cnpjEmUso = await _fornecedorRepositorio.CnpjJaCadastradoAsync(cnpj, id, cancellationToken);

        if (cnpjEmUso)
        {
            throw new ConflitoException($"Já existe outro fornecedor cadastrado com o CNPJ {cnpj}.");
        }

        fornecedor.RazaoSocial = dados.RazaoSocial.Trim();
        fornecedor.NomeFantasia = dados.NomeFantasia.Trim();
        fornecedor.Cnpj = cnpj;
        fornecedor.Email = dados.Email.Trim().ToLower();
        fornecedor.Telefone = dados.Telefone.Trim();
        fornecedor.Cidade = dados.Cidade.Trim();
        fornecedor.Uf = dados.Uf.Trim().ToUpper();
        fornecedor.AtualizadoEm = DateTime.UtcNow;

        await _fornecedorRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorDto.DeModelo(fornecedor);
    }

    public async Task<FornecedorDto> AlterarSituacaoAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken)
    {
        Fornecedor fornecedor = await ObterFornecedorAsync(id, cancellationToken);

        fornecedor.Ativo = ativo;
        fornecedor.AtualizadoEm = DateTime.UtcNow;

        await _fornecedorRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorDto.DeModelo(fornecedor);
    }

    public async Task<FornecedorDto> VincularProdutoAsync(
        int fornecedorId,
        VincularProdutoDto dados,
        CancellationToken cancellationToken)
    {
        Fornecedor fornecedor = await ObterFornecedorAsync(fornecedorId, cancellationToken);

        ProdutoServico? produto = await _produtoRepositorio.ObterPorIdAsync(
            dados.ProdutoServicoId,
            cancellationToken);

        if (produto is null)
        {
            throw new NaoEncontradoException(
                $"O produto {dados.ProdutoServicoId} não foi encontrado.");
        }

        if (produto.Tipo == TipoItem.Servico)
        {
            throw new RegraDeNegocioException(
                "Serviços não podem ser associados a fornecedores.");
        }

        ProdutoFornecedor? vinculo = await _vinculoRepositorio.ObterPrimeiroAsync(
            associacao => associacao.FornecedorId == fornecedorId &&
                          associacao.ProdutoServicoId == dados.ProdutoServicoId,
            cancellationToken);

        if (vinculo is null)
        {
            vinculo = new ProdutoFornecedor
            {
                FornecedorId = fornecedorId,
                ProdutoServicoId = dados.ProdutoServicoId,
                PrecoCusto = decimal.Round(dados.PrecoCusto, 2),
                PrazoEntregaDias = dados.PrazoEntregaDias,
                Preferencial = dados.Preferencial,
                CriadaEm = DateTime.UtcNow
            };

            await _vinculoRepositorio.AdicionarAsync(vinculo, cancellationToken);
        }
        else
        {
            vinculo.PrecoCusto = decimal.Round(dados.PrecoCusto, 2);
            vinculo.PrazoEntregaDias = dados.PrazoEntregaDias;
            vinculo.Preferencial = dados.Preferencial;
        }

        await _vinculoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        Fornecedor? atualizado = await _fornecedorRepositorio.ObterComProdutosAsync(
            fornecedorId,
            cancellationToken);

        return FornecedorDto.DeModelo(atualizado ?? fornecedor);
    }

    public async Task<FornecedorDto> DesvincularProdutoAsync(
        int fornecedorId,
        int produtoId,
        CancellationToken cancellationToken)
    {
        Fornecedor fornecedor = await ObterFornecedorAsync(fornecedorId, cancellationToken);

        ProdutoFornecedor? vinculo = await _vinculoRepositorio.ObterPrimeiroAsync(
            associacao => associacao.FornecedorId == fornecedorId &&
                          associacao.ProdutoServicoId == produtoId,
            cancellationToken);

        if (vinculo is null)
        {
            throw new NaoEncontradoException(
                $"O produto {produtoId} não está associado ao fornecedor {fornecedorId}.");
        }

        _vinculoRepositorio.Remover(vinculo);
        await _vinculoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        Fornecedor? atualizado = await _fornecedorRepositorio.ObterComProdutosAsync(
            fornecedorId,
            cancellationToken);

        return FornecedorDto.DeModelo(atualizado ?? fornecedor);
    }

    private async Task<Fornecedor> ObterFornecedorAsync(int id, CancellationToken cancellationToken)
    {
        Fornecedor? fornecedor = await _fornecedorRepositorio.ObterComProdutosAsync(id, cancellationToken);

        if (fornecedor is null)
        {
            throw new NaoEncontradoException($"O fornecedor {id} não foi encontrado.");
        }

        return fornecedor;
    }
}
