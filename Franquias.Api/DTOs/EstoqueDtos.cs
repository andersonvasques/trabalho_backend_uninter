using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs;

/// <summary>
/// Saldo de um produto no estoque de uma unidade.
/// </summary>
public sealed class EstoqueDto
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public string? UnidadeNome { get; set; }

    public int ProdutoServicoId { get; set; }

    public string ProdutoSku { get; set; } = string.Empty;

    public string ProdutoNome { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public int QuantidadeMinima { get; set; }

    public bool AbaixoDoMinimo { get; set; }

    public DateTime AtualizadoEm { get; set; }

    public static EstoqueDto DeModelo(EstoqueUnidade estoque)
    {
        return new EstoqueDto
        {
            Id = estoque.Id,
            UnidadeFranqueadaId = estoque.UnidadeFranqueadaId,
            UnidadeNome = estoque.Unidade?.NomeFantasia,
            ProdutoServicoId = estoque.ProdutoServicoId,
            ProdutoSku = estoque.ProdutoServico?.Sku ?? string.Empty,
            ProdutoNome = estoque.ProdutoServico?.Nome ?? string.Empty,
            Quantidade = estoque.Quantidade,
            QuantidadeMinima = estoque.QuantidadeMinima,
            AbaixoDoMinimo = estoque.EstaAbaixoDoMinimo(),
            AtualizadoEm = estoque.AtualizadoEm
        };
    }
}

/// <summary>
/// Dados para registrar uma entrada ou saída manual de estoque.
/// </summary>
public sealed class MovimentarEstoqueDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe a unidade da movimentação.")]
    public int UnidadeFranqueadaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe o produto movimentado.")]
    public int ProdutoServicoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }

    [Required(ErrorMessage = "Informe o tipo da movimentação (Entrada, Saida ou Ajuste).")]
    [EnumDataType(typeof(TipoMovimentacaoEstoque), ErrorMessage = "Tipo de movimentação inválido.")]
    public TipoMovimentacaoEstoque Tipo { get; set; }

    [StringLength(300, ErrorMessage = "O motivo deve ter no máximo 300 caracteres.")]
    public string Motivo { get; set; } = string.Empty;
}

/// <summary>
/// Dados para definir o estoque mínimo de um produto em uma unidade.
/// </summary>
public sealed class DefinirEstoqueMinimoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe a unidade.")]
    public int UnidadeFranqueadaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe o produto.")]
    public int ProdutoServicoId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo não pode ser negativo.")]
    public int QuantidadeMinima { get; set; }
}

/// <summary>
/// Histórico de uma movimentação de estoque.
/// </summary>
public sealed class MovimentacaoEstoqueDto
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public int ProdutoServicoId { get; set; }

    public string ProdutoNome { get; set; } = string.Empty;

    public TipoMovimentacaoEstoque Tipo { get; set; }

    public int Quantidade { get; set; }

    public int SaldoAnterior { get; set; }

    public int SaldoAtual { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public int? VendaId { get; set; }

    public DateTime OcorridaEm { get; set; }

    public static MovimentacaoEstoqueDto DeModelo(MovimentacaoEstoque movimentacao)
    {
        return new MovimentacaoEstoqueDto
        {
            Id = movimentacao.Id,
            UnidadeFranqueadaId = movimentacao.EstoqueUnidade?.UnidadeFranqueadaId ?? 0,
            ProdutoServicoId = movimentacao.EstoqueUnidade?.ProdutoServicoId ?? 0,
            ProdutoNome = movimentacao.EstoqueUnidade?.ProdutoServico?.Nome ?? string.Empty,
            Tipo = movimentacao.Tipo,
            Quantidade = movimentacao.Quantidade,
            SaldoAnterior = movimentacao.SaldoAnterior,
            SaldoAtual = movimentacao.SaldoAtual,
            Motivo = movimentacao.Motivo,
            VendaId = movimentacao.VendaId,
            OcorridaEm = movimentacao.OcorridaEm
        };
    }
}

/// <summary>
/// Filtros aceitos na consulta de estoque.
/// </summary>
public sealed class ConsultaEstoqueDto : ParametrosDeConsulta
{
    /// <summary>Unidade consultada. Obrigatória para perfis de unidade.</summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>Texto pesquisado no nome ou SKU do produto.</summary>
    public string? Busca { get; set; }

    /// <summary>Quando verdadeiro, retorna apenas os itens abaixo do estoque mínimo.</summary>
    public bool ApenasAbaixoDoMinimo { get; set; }
}

/// <summary>
/// Filtros aceitos na consulta do histórico de movimentações.
/// </summary>
public sealed class ConsultaMovimentacoesDto : ParametrosDeConsulta
{
    public int? UnidadeFranqueadaId { get; set; }

    public int? ProdutoServicoId { get; set; }

    public TipoMovimentacaoEstoque? Tipo { get; set; }

    public DateTime? DataInicial { get; set; }

    public DateTime? DataFinal { get; set; }
}
