using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de registro e consulta das vendas das unidades.
/// </summary>
public interface IVendaService
{
    Task<ResultadoPaginado<VendaDto>> ListarAsync(
        ConsultaVendasDto filtros,
        CancellationToken cancellationToken);

    Task<VendaDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Registra uma venda, calcula o valor total a partir dos itens
    /// e dá baixa no estoque da unidade.
    /// </summary>
    Task<VendaDto> RegistrarAsync(CriarVendaDto dados, CancellationToken cancellationToken);

    /// <summary>
    /// Cancela a venda, mantém o histórico e devolve os itens ao estoque.
    /// </summary>
    Task<VendaDto> CancelarAsync(
        int id,
        CancelarVendaDto dados,
        CancellationToken cancellationToken);
}
