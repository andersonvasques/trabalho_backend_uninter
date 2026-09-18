using System.Diagnostics;

namespace Franquias.Api.Middleware;

/// <summary>
/// Middleware de log que registra o início e o fim de cada requisição,
/// junto com o status devolvido e o tempo total de processamento.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate proximo,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        long inicio = Stopwatch.GetTimestamp();

        _logger.LogInformation(
            "Iniciando {Metodo} {Caminho}",
            contexto.Request.Method,
            contexto.Request.Path);

        try
        {
            await _proximo(contexto);
        }
        finally
        {
            TimeSpan duracao = Stopwatch.GetElapsedTime(inicio);

            _logger.LogInformation(
                "Finalizando {Metodo} {Caminho} com status {Status} em {Milissegundos:F2} ms",
                contexto.Request.Method,
                contexto.Request.Path,
                contexto.Response.StatusCode,
                duracao.TotalMilliseconds);
        }
    }
}
