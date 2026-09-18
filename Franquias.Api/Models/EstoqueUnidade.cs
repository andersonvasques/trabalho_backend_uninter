using Franquias.Api.Common;

namespace Franquias.Api.Models;

/// <summary>
/// Saldo de um produto no estoque de uma unidade franqueada.
/// Cada par (unidade, produto) possui um único registro de saldo.
/// </summary>
public sealed class EstoqueUnidade
{
    /// <summary>Identificador único do saldo.</summary>
    public int Id { get; set; }

    /// <summary>Unidade dona do estoque.</summary>
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Dados da unidade.</summary>
    public UnidadeFranqueada Unidade { get; set; } = null!;

    /// <summary>Produto controlado.</summary>
    public int ProdutoServicoId { get; set; }

    /// <summary>Dados do produto.</summary>
    public ProdutoServico ProdutoServico { get; set; } = null!;

    /// <summary>Quantidade disponível.</summary>
    public int Quantidade { get; set; }

    /// <summary>Quantidade mínima desejada para a unidade.</summary>
    public int QuantidadeMinima { get; set; }

    /// <summary>Data da última movimentação registrada.</summary>
    public DateTime AtualizadoEm { get; set; }

    /// <summary>Movimentações que geraram o saldo atual.</summary>
    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();

    /// <summary>
    /// Indica se o saldo está igual ou abaixo do mínimo configurado.
    /// </summary>
    public bool EstaAbaixoDoMinimo()
    {
        return Quantidade <= QuantidadeMinima;
    }

    /// <summary>
    /// Adiciona quantidade ao saldo (entrada ou devolução).
    /// </summary>
    public void Creditar(int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new RegraDeNegocioException("A quantidade movimentada deve ser maior que zero.");
        }

        Quantidade += quantidade;
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Retira quantidade do saldo.
    /// Regra de negócio: o estoque nunca pode ficar negativo.
    /// </summary>
    public void Debitar(int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new RegraDeNegocioException("A quantidade movimentada deve ser maior que zero.");
        }

        if (quantidade > Quantidade)
        {
            throw new RegraDeNegocioException(
                $"Estoque insuficiente para o produto '{ProdutoServico?.Nome ?? ProdutoServicoId.ToString()}'. " +
                $"Saldo atual: {Quantidade}. Quantidade solicitada: {quantidade}.");
        }

        Quantidade -= quantidade;
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Ajusta o saldo para um valor informado no inventário.
    /// </summary>
    public void Ajustar(int novaQuantidade)
    {
        if (novaQuantidade < 0)
        {
            throw new RegraDeNegocioException("O estoque não pode ficar negativo.");
        }

        Quantidade = novaQuantidade;
        AtualizadoEm = DateTime.UtcNow;
    }
}
