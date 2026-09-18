using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de cadastro e manutenção das unidades franqueadas.
/// </summary>
public interface IUnidadeService
{
    Task<ResultadoPaginado<UnidadeDto>> ListarAsync(
        ConsultaUnidadesDto filtros,
        CancellationToken cancellationToken);

    Task<UnidadeDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<UnidadeDto> CriarAsync(CriarUnidadeDto dados, CancellationToken cancellationToken);

    Task<UnidadeDto> AtualizarAsync(
        int id,
        AtualizarUnidadeDto dados,
        CancellationToken cancellationToken);

    /// <summary>
    /// Inativa a unidade preservando o histórico de vendas, estoque e chamados.
    /// </summary>
    Task<UnidadeDto> InativarAsync(int id, CancellationToken cancellationToken);

    /// <summary>Reativa uma unidade inativa.</summary>
    Task<UnidadeDto> ReativarAsync(int id, CancellationToken cancellationToken);
}
