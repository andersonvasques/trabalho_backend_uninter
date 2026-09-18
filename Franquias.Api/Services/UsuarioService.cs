using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras de cadastro de usuários.
/// </summary>
public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IUnidadeRepositorio _unidadeRepositorio;
    private readonly IUsuarioContexto _usuarioContexto;

    public UsuarioService(
        IUsuarioRepositorio usuarioRepositorio,
        IUnidadeRepositorio unidadeRepositorio,
        IUsuarioContexto usuarioContexto)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _unidadeRepositorio = unidadeRepositorio;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<ResultadoPaginado<UsuarioDto>> ListarAsync(
        ConsultaUsuariosDto filtros,
        CancellationToken cancellationToken)
    {
        // Gestores enxergam apenas os usuários da própria unidade.
        filtros.UnidadeFranqueadaId = _usuarioContexto.ResolverFiltroDeUnidade(filtros.UnidadeFranqueadaId);

        ResultadoPaginado<Usuario> pagina = await _usuarioRepositorio.ListarPaginadoAsync(
            filtros,
            cancellationToken);

        var itens = new List<UsuarioDto>();

        foreach (Usuario usuario in pagina.Itens)
        {
            itens.Add(UsuarioDto.DeModelo(usuario));
        }

        return new ResultadoPaginado<UsuarioDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<UsuarioDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        Usuario usuario = await ObterUsuarioAsync(id, cancellationToken);

        if (usuario.UnidadeFranqueadaId.HasValue)
        {
            _usuarioContexto.GarantirAcessoAUnidade(usuario.UnidadeFranqueadaId.Value);
        }
        else if (!_usuarioContexto.EhAdministrador)
        {
            throw new AcessoNegadoException(
                "Somente administradores podem consultar usuários da franqueadora.");
        }

        return UsuarioDto.DeModelo(usuario);
    }

    public async Task<UsuarioDto> CriarAsync(
        CriarUsuarioDto dados,
        CancellationToken cancellationToken)
    {
        string email = dados.Email.Trim().ToLower();

        // Regra de negócio: não é permitido cadastrar usuários com e-mail duplicado.
        bool emailEmUso = await _usuarioRepositorio.EmailJaCadastradoAsync(email, null, cancellationToken);

        if (emailEmUso)
        {
            throw new ConflitoException($"Já existe um usuário cadastrado com o e-mail {email}.");
        }

        await ValidarUnidadeDoPerfilAsync(dados.Perfil, dados.UnidadeFranqueadaId, cancellationToken);

        (string hash, string salt) = SegurancaDeSenha.GerarHash(dados.Senha);

        var usuario = new Usuario
        {
            Nome = dados.Nome.Trim(),
            Email = email,
            SenhaHash = hash,
            SenhaSalt = salt,
            Perfil = dados.Perfil,
            UnidadeFranqueadaId = dados.Perfil == PerfilUsuario.Administrador
                ? null
                : dados.UnidadeFranqueadaId,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        await _usuarioRepositorio.AdicionarAsync(usuario, cancellationToken);
        await _usuarioRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioDto.DeModelo(usuario);
    }

    public async Task<UsuarioDto> AtualizarAsync(
        int id,
        AtualizarUsuarioDto dados,
        CancellationToken cancellationToken)
    {
        Usuario usuario = await ObterUsuarioAsync(id, cancellationToken);

        string email = dados.Email.Trim().ToLower();

        bool emailEmUso = await _usuarioRepositorio.EmailJaCadastradoAsync(email, id, cancellationToken);

        if (emailEmUso)
        {
            throw new ConflitoException($"Já existe outro usuário cadastrado com o e-mail {email}.");
        }

        await ValidarUnidadeDoPerfilAsync(dados.Perfil, dados.UnidadeFranqueadaId, cancellationToken);

        usuario.Nome = dados.Nome.Trim();
        usuario.Email = email;
        usuario.Perfil = dados.Perfil;
        usuario.UnidadeFranqueadaId = dados.Perfil == PerfilUsuario.Administrador
            ? null
            : dados.UnidadeFranqueadaId;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _usuarioRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioDto.DeModelo(usuario);
    }

    public async Task<UsuarioDto> AlterarSituacaoAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken)
    {
        Usuario usuario = await ObterUsuarioAsync(id, cancellationToken);

        if (!ativo && usuario.Id == _usuarioContexto.UsuarioId)
        {
            throw new RegraDeNegocioException("Não é possível inativar o próprio usuário.");
        }

        usuario.Ativo = ativo;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _usuarioRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioDto.DeModelo(usuario);
    }

    private async Task<Usuario> ObterUsuarioAsync(int id, CancellationToken cancellationToken)
    {
        Usuario? usuario = await _usuarioRepositorio.ObterComUnidadeAsync(id, cancellationToken);

        if (usuario is null)
        {
            throw new NaoEncontradoException($"O usuário {id} não foi encontrado.");
        }

        return usuario;
    }

    /// <summary>
    /// Regra de negócio: administradores pertencem à franqueadora e não possuem
    /// unidade; gestores e operadores precisam estar vinculados a uma unidade válida.
    /// </summary>
    private async Task ValidarUnidadeDoPerfilAsync(
        PerfilUsuario perfil,
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        if (perfil == PerfilUsuario.Administrador)
        {
            return;
        }

        if (unidadeId is null)
        {
            throw new RegraDeNegocioException(
                "Usuários com perfil Gestor ou Operador precisam estar vinculados a uma unidade.");
        }

        UnidadeFranqueada? unidade = await _unidadeRepositorio.ObterPorIdAsync(
            unidadeId.Value,
            cancellationToken);

        if (unidade is null)
        {
            throw new NaoEncontradoException($"A unidade {unidadeId} não foi encontrada.");
        }
    }
}
