using Franquias.Api.Common;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação do repositório de usuários.
/// </summary>
public sealed class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
{
    public UsuarioRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    public async Task<ResultadoPaginado<Usuario>> ListarPaginadoAsync(
        ConsultaUsuariosDto filtros,
        CancellationToken cancellationToken)
    {
        IQueryable<Usuario> consulta = Conjunto
            .AsNoTracking()
            .Include(usuario => usuario.Unidade);

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim();

            consulta = consulta.Where(usuario =>
                usuario.Nome.Contains(termo) ||
                usuario.Email.Contains(termo));
        }

        if (filtros.Perfil.HasValue)
        {
            consulta = consulta.Where(usuario => usuario.Perfil == filtros.Perfil.Value);
        }

        if (filtros.Ativo.HasValue)
        {
            consulta = consulta.Where(usuario => usuario.Ativo == filtros.Ativo.Value);
        }

        if (filtros.UnidadeFranqueadaId.HasValue)
        {
            consulta = consulta.Where(usuario =>
                usuario.UnidadeFranqueadaId == filtros.UnidadeFranqueadaId.Value);
        }

        int total = await consulta.CountAsync(cancellationToken);

        consulta = filtros.Decrescente
            ? consulta.OrderByDescending(usuario => usuario.Nome)
            : consulta.OrderBy(usuario => usuario.Nome);

        List<Usuario> itens = await consulta
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Usuario>(
            itens,
            total,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken)
    {
        string emailNormalizado = email.Trim().ToLower();

        return await Conjunto
            .Include(usuario => usuario.Unidade)
            .FirstOrDefaultAsync(
                usuario => usuario.Email.ToLower() == emailNormalizado,
                cancellationToken);
    }

    public async Task<bool> EmailJaCadastradoAsync(
        string email,
        int? idIgnorado,
        CancellationToken cancellationToken)
    {
        string emailNormalizado = email.Trim().ToLower();

        return await Conjunto
            .AsNoTracking()
            .AnyAsync(
                usuario => usuario.Email.ToLower() == emailNormalizado &&
                           (idIgnorado == null || usuario.Id != idIgnorado),
                cancellationToken);
    }

    public async Task<Usuario?> ObterComUnidadeAsync(int id, CancellationToken cancellationToken)
    {
        return await Conjunto
            .Include(usuario => usuario.Unidade)
            .FirstOrDefaultAsync(usuario => usuario.Id == id, cancellationToken);
    }
}
