using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs;

/// <summary>
/// Dados necessários para registrar uma venda.
/// </summary>
public sealed class CriarVendaDto
{
    /// <summary>
    /// Unidade que está vendendo. Gestores e operadores só podem
    /// registrar vendas para a própria unidade.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe a unidade que está registrando a venda.")]
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Data da venda. Quando não informada, usa a data atual.</summary>
    public DateTime? DataVenda { get; set; }

    [EnumDataType(typeof(FormaPagamento), ErrorMessage = "Forma de pagamento inválida.")]
    public FormaPagamento FormaPagamento { get; set; } = FormaPagamento.Dinheiro;

    [Range(0, 1000000, ErrorMessage = "O desconto não pode ser negativo.")]
    public decimal Desconto { get; set; }

    [StringLength(150, ErrorMessage = "O nome do cliente deve ter no máximo 150 caracteres.")]
    public string? Cliente { get; set; }

    [StringLength(300, ErrorMessage = "A observação deve ter no máximo 300 caracteres.")]
    public string? Observacao { get; set; }

    /// <summary>Itens vendidos. A venda precisa de pelo menos um item.</summary>
    [Required(ErrorMessage = "A venda deve possuir pelo menos um item.")]
    [MinLength(1, ErrorMessage = "A venda deve possuir pelo menos um item.")]
    public List<CriarItemVendaDto> Itens { get; set; } = new List<CriarItemVendaDto>();
}

/// <summary>
/// Item informado no registro de uma venda.
/// </summary>
public sealed class CriarItemVendaDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o produto ou serviço vendido.")]
    public int ProdutoServicoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }

    /// <summary>
    /// Preço praticado. Quando não informado, a API utiliza
    /// o preço base cadastrado para o item.
    /// </summary>
    [Range(0, 1000000, ErrorMessage = "O preço unitário não pode ser negativo.")]
    public decimal? PrecoUnitario { get; set; }
}

/// <summary>
/// Dados para cancelamento de uma venda.
/// </summary>
public sealed class CancelarVendaDto
{
    [Required(ErrorMessage = "Informe o motivo do cancelamento.")]
    [StringLength(300, MinimumLength = 5, ErrorMessage = "O motivo deve ter entre 5 e 300 caracteres.")]
    public string Motivo { get; set; } = string.Empty;
}

/// <summary>
/// Representação de uma venda devolvida pela API.
/// </summary>
public sealed class VendaDto
{
    public int Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int UnidadeFranqueadaId { get; set; }

    public string? UnidadeNome { get; set; }

    public int UsuarioId { get; set; }

    public string? UsuarioNome { get; set; }

    public DateTime DataVenda { get; set; }

    public StatusVenda Status { get; set; }

    public FormaPagamento FormaPagamento { get; set; }

    public decimal Desconto { get; set; }

    public decimal ValorTotal { get; set; }

    public string? Cliente { get; set; }

    public string? Observacao { get; set; }

    public DateTime? CanceladaEm { get; set; }

    public string? MotivoCancelamento { get; set; }

    public IReadOnlyCollection<ItemVendaDto> Itens { get; set; } = new List<ItemVendaDto>();

    public static VendaDto DeModelo(Venda venda)
    {
        var itens = new List<ItemVendaDto>();

        foreach (ItemVenda item in venda.Itens)
        {
            itens.Add(ItemVendaDto.DeModelo(item));
        }

        return new VendaDto
        {
            Id = venda.Id,
            Numero = venda.Numero,
            UnidadeFranqueadaId = venda.UnidadeFranqueadaId,
            UnidadeNome = venda.Unidade?.NomeFantasia,
            UsuarioId = venda.UsuarioId,
            UsuarioNome = venda.Usuario?.Nome,
            DataVenda = venda.DataVenda,
            Status = venda.Status,
            FormaPagamento = venda.FormaPagamento,
            Desconto = venda.Desconto,
            ValorTotal = venda.ValorTotal,
            Cliente = venda.Cliente,
            Observacao = venda.Observacao,
            CanceladaEm = venda.CanceladaEm,
            MotivoCancelamento = venda.MotivoCancelamento,
            Itens = itens
        };
    }
}

/// <summary>
/// Item de uma venda devolvido pela API.
/// </summary>
public sealed class ItemVendaDto
{
    public int Id { get; set; }

    public int ProdutoServicoId { get; set; }

    public string DescricaoItem { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public static ItemVendaDto DeModelo(ItemVenda item)
    {
        return new ItemVendaDto
        {
            Id = item.Id,
            ProdutoServicoId = item.ProdutoServicoId,
            DescricaoItem = item.DescricaoItem,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario,
            Subtotal = item.Subtotal
        };
    }
}

/// <summary>
/// Filtros aceitos na listagem de vendas.
/// Exemplo: GET /api/vendas?unidadeFranqueadaId=1&amp;dataInicial=2026-01-01
/// </summary>
public sealed class ConsultaVendasDto : ParametrosDeConsulta
{
    public int? UnidadeFranqueadaId { get; set; }

    public DateTime? DataInicial { get; set; }

    public DateTime? DataFinal { get; set; }

    public StatusVenda? Status { get; set; }

    public FormaPagamento? FormaPagamento { get; set; }

    /// <summary>Texto pesquisado no número da venda ou no nome do cliente.</summary>
    public string? Busca { get; set; }
}
