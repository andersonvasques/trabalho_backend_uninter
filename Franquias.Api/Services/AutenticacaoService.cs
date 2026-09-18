using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras de autenticação.
/// </summary>
public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IGeradorDeToken _geradorDeToken;
    private readonly IUsuarioContexto _usuarioContexto;

    public AutenticacaoService(
        IUsuarioRepositorio usuarioRepositorio,
        IGeradorDeToken geradorDeToken,
        IUsuarioContexto usuarioContexto)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _geradorDeToken = geradorDeToken;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<LoginRespostaDto> AutenticarAsync(
        LoginDto dados,
        CancellationToken cancellationToken)
    {
        Usuario? usuario = await _usuarioRepositorio.ObterPorEmailAsync(dados.Email, cancellationToken);

        // A mesma mensagem é usada para e-mail inexistente e senha incorreta,
        // evitando informar a um possível invasor quais e-mails existem.
        if (usuario is null)
        {
            throw new NaoAutorizadoException("E-mail ou senha inválidos.");
        }

        if (!SegurancaDeSenha.Conferir(dados.Senha, usuario.SenhaHash, usuario.SenhaSalt))
        {
            throw new NaoAutorizadoException("E-mail ou senha inválidos.");
        }

        if (!usuario.Ativo)
        {
            throw new AcessoNegadoException("Usuário inativo. Procure o administrador da rede.");
        }

        usuario.UltimoAcessoEm = DateTime.UtcNow;
        await _usuarioRepositorio.SalvarAlteracoesAsync(cancellationToken);

        TokenGerado token = _geradorDeToken.Gerar(usuario);

        return new LoginRespostaDto
        {
            Token = token.Token,
            ExpiraEm = token.ExpiraEm,
            Usuario = UsuarioDto.DeModelo(usuario)
        };
    }

    public async Task<UsuarioDto> ObterUsuarioAutenticadoAsync(CancellationToken cancellationToken)
    {
        Usuario? usuario = await _usuarioRepositorio.ObterComUnidadeAsync(
            _usuarioContexto.UsuarioId,
            cancellationToken);

        if (usuario is null)
        {
            throw new NaoEncontradoException("O usuário autenticado não foi encontrado.");
        }

        return UsuarioDto.DeModelo(usuario);
    }

    public async Task AlterarSenhaAsync(AlterarSenhaDto dados, CancellationToken cancellationToken)
    {
        Usuario? usuario = await _usuarioRepositorio.ObterPorIdAsync(
            _usuarioContexto.UsuarioId,
            cancellationToken);

        if (usuario is null)
        {
            throw new NaoEncontradoException("O usuário autenticado não foi encontrado.");
        }

        if (!SegurancaDeSenha.Conferir(dados.SenhaAtual, usuario.SenhaHash, usuario.SenhaSalt))
        {
            throw new RegraDeNegocioException("A senha atual informada está incorreta.");
        }

        (string hash, string salt) = SegurancaDeSenha.GerarHash(dados.NovaSenha);

        usuario.SenhaHash = hash;
        usuario.SenhaSalt = salt;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _usuarioRepositorio.SalvarAlteracoesAsync(cancellationToken);
    }
}
