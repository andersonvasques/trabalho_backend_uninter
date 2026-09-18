using System.Text.Json;
using Franquias.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Middleware;

/// <summary>
/// Middleware que centraliza o tratamento de exceções da API.
///
/// As exceções de negócio são convertidas em respostas HTTP coerentes
/// (400, 401, 403, 404, 409) e qualquer erro inesperado vira um 500
/// com mensagem genérica, sem expor detalhes internos ao cliente.
/// </summary>
public sealed class TratamentoDeErrosMiddleware
{
    private static readonly JsonSerializerOptions OpcoesJson = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _proximo;
    private readonly ILogger<TratamentoDeErrosMiddleware> _logger;

    public TratamentoDeErrosMiddleware(
        RequestDelegate proximo,
        ILogger<TratamentoDeErrosMiddleware> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (ExcecaoDaAplicacao excecao)
        {
            _logger.LogWarning(
                "Regra da aplicação acionada em {Caminho}: {Mensagem}",
                contexto.Request.Path,
                excecao.Message);

            await EscreverRespostaAsync(
                contexto,
                excecao.StatusHttp,
                excecao.Titulo,
                excecao.Message);
        }
        catch (Exception excecao)
        {
            _logger.LogError(
                excecao,
                "Erro inesperado ao processar {Metodo} {Caminho}.",
                contexto.Request.Method,
                contexto.Request.Path);

            await EscreverRespostaAsync(
                contexto,
                StatusCodes.Status500InternalServerError,
                "Erro interno",
                "Ocorreu um erro inesperado ao processar a requisição.");
        }
    }

    private static async Task EscreverRespostaAsync(
        HttpContext contexto,
        int status,
        string titulo,
        string detalhe)
    {
        if (contexto.Response.HasStarted)
        {
            return;
        }

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe,
            Instance = contexto.Request.Path
        };

        contexto.Response.Clear();
        contexto.Response.StatusCode = status;
        contexto.Response.ContentType = "application/problem+json";

        await contexto.Response.WriteAsync(JsonSerializer.Serialize(problema, OpcoesJson));
    }
}
