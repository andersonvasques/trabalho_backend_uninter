using Franquias.Api.Common;
using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de cadastro e manutenção dos usuários do sistema.
/// </summary>
public interface IUsuarioService
{
    Task<ResultadoPaginado<UsuarioDto>> ListarAsync(
        ConsultaUsuariosDto filtros,
        CancellationToken cancellationToken);

    Task<UsuarioDto> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<UsuarioDto> CriarAsync(CriarUsuarioDto dados, CancellationToken cancellationToken);

    Task<UsuarioDto> AtualizarAsync(
        int id,
        AtualizarUsuarioDto dados,
        CancellationToken cancellationToken);

    /// <summary>Ativa ou inativa o usuário (exclusão lógica).</summary>
    Task<UsuarioDto> AlterarSituacaoAsync(int id, bool ativo, CancellationToken cancellationToken);
}
