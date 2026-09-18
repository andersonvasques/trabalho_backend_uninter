using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

/// <summary>
/// Operações de acesso a dados específicas dos usuários.
/// </summary>
public interface IUsuarioRepositorio : IRepositorioBase<Usuario>
{
    /// <summary>Lista os usuários com filtros, ordenação e paginação.</summary>
    Task<ResultadoPaginado<Usuario>> ListarPaginadoAsync(
        ConsultaUsuariosDto filtros,
        CancellationToken cancellationToken);

    /// <summary>Busca um usuário pelo e-mail (usado no login).</summary>
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>Indica se já existe outro usuário com o e-mail informado.</summary>
    Task<bool> EmailJaCadastradoAsync(string email, int? idIgnorado, CancellationToken cancellationToken);

    /// <summary>Obtém o usuário com os dados da unidade vinculada.</summary>
    Task<Usuario?> ObterComUnidadeAsync(int id, CancellationToken cancellationToken);
}
