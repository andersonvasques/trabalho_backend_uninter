namespace Franquias.Api.Models;

/// <summary>
/// Agrupamento dos itens do catálogo padronizado da rede
/// (ex.: Bebidas, Alimentos, Serviços).
/// </summary>
public sealed class Categoria
{
    /// <summary>Identificador único da categoria.</summary>
    public int Id { get; set; }

    /// <summary>Nome da categoria. Não pode se repetir.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Descrição livre da categoria.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Indica se a categoria pode receber novos itens.</summary>
    public bool Ativa { get; set; } = true;

    /// <summary>Data de criação do cadastro.</summary>
    public DateTime CriadaEm { get; set; }

    /// <summary>Data da última alteração do cadastro.</summary>
    public DateTime? AtualizadaEm { get; set; }

    /// <summary>Itens pertencentes à categoria.</summary>
    public ICollection<ProdutoServico> Itens { get; set; } = new List<ProdutoServico>();
}
