using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.Common;

/// <summary>
/// Parâmetros comuns de paginação e ordenação utilizados nas consultas.
/// São recebidos via query string, por exemplo:
/// GET /api/unidades?pagina=1&amp;tamanhoPagina=10&amp;ordenarPor=nome&amp;decrescente=false
/// </summary>
public class ParametrosDeConsulta
{
    private const int TamanhoMaximoDaPagina = 100;

    private int _pagina = 1;
    private int _tamanhoPagina = 20;

    /// <summary>Número da página desejada (começa em 1).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior ou igual a 1.")]
    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    /// <summary>Quantidade de registros por página (máximo de 100).</summary>
    [Range(1, TamanhoMaximoDaPagina, ErrorMessage = "O tamanho da página deve estar entre 1 e 100.")]
    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set
        {
            if (value < 1)
            {
                _tamanhoPagina = 1;
                return;
            }

            _tamanhoPagina = value > TamanhoMaximoDaPagina ? TamanhoMaximoDaPagina : value;
        }
    }

    /// <summary>Campo utilizado na ordenação. Cada consulta documenta os valores aceitos.</summary>
    public string? OrdenarPor { get; set; }

    /// <summary>Quando verdadeiro, a ordenação é decrescente.</summary>
    public bool Decrescente { get; set; }

    /// <summary>Quantidade de registros que devem ser ignorados na consulta.</summary>
    public int RegistrosParaPular()
    {
        return (Pagina - 1) * TamanhoPagina;
    }
}

/// <summary>
/// Envelope devolvido pelas consultas paginadas.
/// </summary>
/// <typeparam name="T">Tipo do item retornado.</typeparam>
public sealed class ResultadoPaginado<T>
{
    public ResultadoPaginado(IReadOnlyCollection<T> itens, int totalItens, int pagina, int tamanhoPagina)
    {
        Itens = itens;
        TotalItens = totalItens;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
    }

    /// <summary>Registros da página atual.</summary>
    public IReadOnlyCollection<T> Itens { get; }

    /// <summary>Total de registros encontrados pelo filtro.</summary>
    public int TotalItens { get; }

    /// <summary>Página atual.</summary>
    public int Pagina { get; }

    /// <summary>Quantidade de registros por página.</summary>
    public int TamanhoPagina { get; }

    /// <summary>Quantidade total de páginas disponíveis.</summary>
    public int TotalPaginas => TamanhoPagina == 0
        ? 0
        : (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);

    /// <summary>Indica se existe uma próxima página.</summary>
    public bool PossuiProximaPagina => Pagina < TotalPaginas;

    /// <summary>Indica se existe uma página anterior.</summary>
    public bool PossuiPaginaAnterior => Pagina > 1;
}
