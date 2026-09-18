namespace Franquias.Api.Models;

/// <summary>
/// Perfis de acesso disponíveis no sistema.
/// O perfil define quais operações o usuário pode executar.
/// </summary>
public enum PerfilUsuario
{
    /// <summary>Usuário da franqueadora: enxerga e administra toda a rede.</summary>
    Administrador = 1,

    /// <summary>Gestor de uma unidade franqueada: administra apenas a própria unidade.</summary>
    Gestor = 2,

    /// <summary>Operador da unidade: executa vendas e movimentações do dia a dia.</summary>
    Operador = 3
}

/// <summary>
/// Situação contratual de uma unidade franqueada.
/// </summary>
public enum SituacaoUnidade
{
    EmImplantacao = 1,
    Ativa = 2,
    Suspensa = 3,
    Inativa = 4
}

/// <summary>
/// Indica se o item do catálogo é um produto físico ou um serviço.
/// Serviços não controlam estoque.
/// </summary>
public enum TipoItem
{
    Produto = 1,
    Servico = 2
}

/// <summary>
/// Tipo de movimentação registrada no estoque da unidade.
/// </summary>
public enum TipoMovimentacaoEstoque
{
    Entrada = 1,
    Saida = 2,
    Ajuste = 3
}

/// <summary>
/// Situação de uma venda registrada por uma unidade.
/// </summary>
public enum StatusVenda
{
    Confirmada = 1,
    Cancelada = 2
}

/// <summary>
/// Formas de pagamento aceitas pela rede.
/// </summary>
public enum FormaPagamento
{
    Dinheiro = 1,
    Pix = 2,
    CartaoDebito = 3,
    CartaoCredito = 4,
    Boleto = 5
}

/// <summary>
/// Situação da cobrança de royalty de uma unidade em uma competência.
/// </summary>
public enum SituacaoRoyalty
{
    Pendente = 1,
    Pago = 2,
    Atrasado = 3,
    Cancelado = 4
}

/// <summary>
/// Prioridade de atendimento de um chamado aberto pela unidade.
/// </summary>
public enum PrioridadeChamado
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

/// <summary>
/// Situação atual de um chamado de suporte.
/// </summary>
public enum StatusChamado
{
    Aberto = 1,
    EmAtendimento = 2,
    Resolvido = 3,
    Fechado = 4,
    Cancelado = 5
}

/// <summary>
/// Assunto do chamado aberto pela unidade franqueada.
/// </summary>
public enum CategoriaChamado
{
    Sistema = 1,
    Operacional = 2,
    Financeiro = 3,
    Marketing = 4,
    Estoque = 5,
    Outros = 6
}
