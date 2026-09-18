namespace Franquias.Api.Common;

/// <summary>
/// Exceção base da aplicação. Cada exceção derivada carrega o
/// código HTTP que deve ser devolvido ao cliente da API.
/// </summary>
public abstract class ExcecaoDaAplicacao : Exception
{
    protected ExcecaoDaAplicacao(string mensagem, int statusHttp, string titulo)
        : base(mensagem)
    {
        StatusHttp = statusHttp;
        Titulo = titulo;
    }

    /// <summary>Código HTTP correspondente ao erro.</summary>
    public int StatusHttp { get; }

    /// <summary>Título curto exibido na resposta padronizada (ProblemDetails).</summary>
    public string Titulo { get; }
}

/// <summary>
/// Utilizada quando uma regra de negócio é violada (HTTP 400).
/// Exemplo: venda sem itens, estoque insuficiente, CNPJ duplicado.
/// </summary>
public sealed class RegraDeNegocioException : ExcecaoDaAplicacao
{
    public RegraDeNegocioException(string mensagem)
        : base(mensagem, StatusCodes.Status400BadRequest, "Regra de negócio violada")
    {
    }
}

/// <summary>
/// Utilizada quando o recurso solicitado não existe (HTTP 404).
/// </summary>
public sealed class NaoEncontradoException : ExcecaoDaAplicacao
{
    public NaoEncontradoException(string mensagem)
        : base(mensagem, StatusCodes.Status404NotFound, "Recurso não encontrado")
    {
    }

    /// <summary>
    /// Atalho para mensagens padronizadas, como
    /// "A unidade 10 não foi encontrada.".
    /// </summary>
    public static NaoEncontradoException Para(string recurso, int id)
    {
        return new NaoEncontradoException($"{recurso} {id} não foi encontrado(a).");
    }
}

/// <summary>
/// Utilizada quando o usuário autenticado não possui permissão
/// para executar a operação solicitada (HTTP 403).
/// </summary>
public sealed class AcessoNegadoException : ExcecaoDaAplicacao
{
    public AcessoNegadoException(string mensagem)
        : base(mensagem, StatusCodes.Status403Forbidden, "Acesso negado")
    {
    }
}

/// <summary>
/// Utilizada quando as credenciais informadas no login são inválidas (HTTP 401).
/// </summary>
public sealed class NaoAutorizadoException : ExcecaoDaAplicacao
{
    public NaoAutorizadoException(string mensagem)
        : base(mensagem, StatusCodes.Status401Unauthorized, "Não autorizado")
    {
    }
}

/// <summary>
/// Utilizada quando há conflito de dados únicos, como CNPJ ou e-mail
/// já cadastrados (HTTP 409).
/// </summary>
public sealed class ConflitoException : ExcecaoDaAplicacao
{
    public ConflitoException(string mensagem)
        : base(mensagem, StatusCodes.Status409Conflict, "Conflito de dados")
    {
    }
}
