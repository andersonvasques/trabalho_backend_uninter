using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;
using Franquias.Api.Validations;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados de cadastro e alteração de uma categoria.
/// </summary>
public sealed class CategoriaEntradaDto
{
    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
    public string Descricao { get; set; } = string.Empty;
}

/// <summary>
/// Representação de uma categoria devolvida pela API.
/// </summary>
public sealed class CategoriaDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public bool Ativa { get; set; }

    public int QuantidadeDeItens { get; set; }

    public static CategoriaDto DeModelo(Categoria categoria, int quantidadeDeItens = 0)
    {
        return new CategoriaDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            Ativa = categoria.Ativa,
            QuantidadeDeItens = quantidadeDeItens
        };
    }
}

/// <summary>
/// Dados de cadastro de um produto ou serviço.
/// </summary>
public sealed class CriarProdutoDto
{
    [Required(ErrorMessage = "O SKU é obrigatório.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "O SKU deve ter entre 2 e 30 caracteres.")]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Informe a categoria do item.")]
    public int CategoriaId { get; set; }

    [EnumDataType(typeof(TipoItem), ErrorMessage = "Tipo inválido. Use Produto ou Servico.")]
    public TipoItem Tipo { get; set; } = TipoItem.Produto;

    [Range(0.01, 1000000, ErrorMessage = "O preço base deve ser maior que zero.")]
    public decimal PrecoBase { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo não pode ser negativo.")]
    public int EstoqueMinimoPadrao { get; set; }
}

/// <summary>
/// Dados alteráveis de um produto ou serviço.
/// </summary>
public sealed class AtualizarProdutoDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Informe a categoria do item.")]
    public int CategoriaId { get; set; }

    [EnumDataType(typeof(TipoItem), ErrorMessage = "Tipo inválido. Use Produto ou Servico.")]
    public TipoItem Tipo { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "O preço base deve ser maior que zero.")]
    public decimal PrecoBase { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo não pode ser negativo.")]
    public int EstoqueMinimoPadrao { get; set; }
}

/// <summary>
/// Representação de um produto ou serviço devolvida pela API.
/// </summary>
public sealed class ProdutoDto
{
    public int Id { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public int CategoriaId { get; set; }

    public string? CategoriaNome { get; set; }

    public TipoItem Tipo { get; set; }

    public decimal PrecoBase { get; set; }

    public int EstoqueMinimoPadrao { get; set; }

    public bool Ativo { get; set; }

    public DateTime CriadoEm { get; set; }

    public static ProdutoDto DeModelo(ProdutoServico produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Sku = produto.Sku,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            CategoriaId = produto.CategoriaId,
            CategoriaNome = produto.Categoria?.Nome,
            Tipo = produto.Tipo,
            PrecoBase = produto.PrecoBase,
            EstoqueMinimoPadrao = produto.EstoqueMinimoPadrao,
            Ativo = produto.Ativo,
            CriadoEm = produto.CriadoEm
        };
    }
}

/// <summary>
/// Filtros aceitos na listagem de produtos e serviços.
/// </summary>
public sealed class ConsultaProdutosDto : ParametrosDeConsulta
{
    /// <summary>Texto pesquisado no nome, SKU ou descrição.</summary>
    public string? Busca { get; set; }

    /// <summary>Filtra pela categoria.</summary>
    public int? CategoriaId { get; set; }

    /// <summary>Filtra por produto ou serviço.</summary>
    public TipoItem? Tipo { get; set; }

    /// <summary>Filtra por itens ativos ou inativos.</summary>
    public bool? Ativo { get; set; }
}

/// <summary>
/// Dados de cadastro e alteração de um fornecedor.
/// </summary>
public sealed class FornecedorEntradaDto
{
    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "A razão social deve ter entre 3 e 150 caracteres.")]
    public string RazaoSocial { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "O nome fantasia deve ter no máximo 150 caracteres.")]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [Cnpj]
    public string Cnpj { get; set; } = string.Empty;

    [EmailOpcional]
    [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    [UfOpcional]
    public string Uf { get; set; } = string.Empty;
}

/// <summary>
/// Representação de um fornecedor devolvida pela API.
/// </summary>
public sealed class FornecedorDto
{
    public int Id { get; set; }

    public string RazaoSocial { get; set; } = string.Empty;

    public string NomeFantasia { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Uf { get; set; } = string.Empty;

    public bool Ativo { get; set; }

    public IReadOnlyCollection<ProdutoFornecidoDto> Produtos { get; set; } = new List<ProdutoFornecidoDto>();

    public static FornecedorDto DeModelo(Fornecedor fornecedor)
    {
        var produtos = new List<ProdutoFornecidoDto>();

        foreach (ProdutoFornecedor vinculo in fornecedor.Produtos)
        {
            produtos.Add(ProdutoFornecidoDto.DeModelo(vinculo));
        }

        return new FornecedorDto
        {
            Id = fornecedor.Id,
            RazaoSocial = fornecedor.RazaoSocial,
            NomeFantasia = fornecedor.NomeFantasia,
            Cnpj = fornecedor.Cnpj,
            Email = fornecedor.Email,
            Telefone = fornecedor.Telefone,
            Cidade = fornecedor.Cidade,
            Uf = fornecedor.Uf,
            Ativo = fornecedor.Ativo,
            Produtos = produtos
        };
    }
}

/// <summary>
/// Item fornecido por um fornecedor.
/// </summary>
public sealed class ProdutoFornecidoDto
{
    public int ProdutoServicoId { get; set; }

    public string ProdutoNome { get; set; } = string.Empty;

    public decimal PrecoCusto { get; set; }

    public int PrazoEntregaDias { get; set; }

    public bool Preferencial { get; set; }

    public static ProdutoFornecidoDto DeModelo(ProdutoFornecedor vinculo)
    {
        return new ProdutoFornecidoDto
        {
            ProdutoServicoId = vinculo.ProdutoServicoId,
            ProdutoNome = vinculo.ProdutoServico?.Nome ?? string.Empty,
            PrecoCusto = vinculo.PrecoCusto,
            PrazoEntregaDias = vinculo.PrazoEntregaDias,
            Preferencial = vinculo.Preferencial
        };
    }
}

/// <summary>
/// Dados para associar um produto a um fornecedor.
/// </summary>
public sealed class VincularProdutoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o produto que será associado.")]
    public int ProdutoServicoId { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "O preço de custo deve ser maior que zero.")]
    public decimal PrecoCusto { get; set; }

    [Range(0, 365, ErrorMessage = "O prazo de entrega deve estar entre 0 e 365 dias.")]
    public int PrazoEntregaDias { get; set; }

    public bool Preferencial { get; set; }
}

/// <summary>
/// Filtros aceitos na listagem de fornecedores.
/// </summary>
public sealed class ConsultaFornecedoresDto : ParametrosDeConsulta
{
    /// <summary>Texto pesquisado na razão social, nome fantasia ou CNPJ.</summary>
    public string? Busca { get; set; }

    /// <summary>Filtra por fornecedores ativos ou inativos.</summary>
    public bool? Ativo { get; set; }

    /// <summary>Filtra pela cidade do fornecedor.</summary>
    public string? Cidade { get; set; }
}
