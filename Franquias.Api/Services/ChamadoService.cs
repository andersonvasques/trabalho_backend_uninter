using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras dos chamados de suporte.
/// </summary>
public sealed class ChamadoService : IChamadoService
{
    private readonly IChamadoRepositorio _chamadoRepositorio;
    private readonly IUnidadeRepositorio _unidadeRepositorio;
    private readonly IUsuarioContexto _usuarioContexto;

    public ChamadoService(
        IChamadoRepositorio chamadoRepositorio,
        IUnidadeRepositorio unidadeRepositorio,
        IUsuarioContexto usuarioContexto)
    {
        _chamadoRepositorio = chamadoRepositorio;
        _unidadeRepositorio = unidadeRepositorio;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<ResultadoPaginado<ChamadoDto>> ListarAsync(
        ConsultaChamadosDto filtros,
        CancellationToken cancellationToken)
    {
        filtros.UnidadeFranqueadaId = _usuarioContexto.ResolverFiltroDeUnidade(filtros.UnidadeFranqueadaId);

        ResultadoPaginado<ChamadoSuporte> pagina =
            await _chamadoRepositorio.ListarPaginadoAsync(filtros, cancellationToken);

        var itens = new List<ChamadoDto>();

        foreach (ChamadoSuporte chamado in pagina.Itens)
        {
            itens.Add(ChamadoDto.DeModelo(chamado));
        }

        return new ResultadoPaginado<ChamadoDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<ChamadoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        ChamadoSuporte chamado = await ObterChamadoAsync(id, cancellationToken);

        return ChamadoDto.DeModelo(chamado);
    }

    public async Task<ChamadoDto> AbrirAsync(
        AbrirChamadoDto dados,
        CancellationToken cancellationToken)
    {
        _usuarioContexto.GarantirAcessoAUnidade(dados.UnidadeFranqueadaId);

        UnidadeFranqueada? unidade = await _unidadeRepositorio.ObterPorIdAsync(
            dados.UnidadeFranqueadaId,
            cancellationToken);

        if (unidade is null)
        {
            throw new NaoEncontradoException($"A unidade {dados.UnidadeFranqueadaId} não foi encontrada.");
        }

        var chamado = new ChamadoSuporte
        {
            Protocolo = await _chamadoRepositorio.GerarProtocoloAsync(cancellationToken),
            UnidadeFranqueadaId = unidade.Id,
            UsuarioAberturaId = _usuarioContexto.UsuarioId,
            Titulo = dados.Titulo.Trim(),
            Descricao = dados.Descricao.Trim(),
            Categoria = dados.Categoria,
            Prioridade = dados.Prioridade,
            Status = StatusChamado.Aberto,
            AbertoEm = DateTime.UtcNow
        };

        chamado.Interacoes.Add(new ChamadoInteracao
        {
            UsuarioId = _usuarioContexto.UsuarioId,
            Mensagem = "Chamado aberto pela unidade.",
            StatusRegistrado = StatusChamado.Aberto,
            RegistradaEm = DateTime.UtcNow
        });

        await _chamadoRepositorio.AdicionarAsync(chamado, cancellationToken);
        await _chamadoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        ChamadoSuporte? aberto = await _chamadoRepositorio.ObterCompletoAsync(
            chamado.Id,
            cancellationToken);

        return ChamadoDto.DeModelo(aberto ?? chamado);
    }

    public async Task<ChamadoDto> RegistrarInteracaoAsync(
        int id,
        RegistrarInteracaoDto dados,
        CancellationToken cancellationToken)
    {
        ChamadoSuporte chamado = await ObterChamadoAsync(id, cancellationToken);

        if (!chamado.EstaEmAberto())
        {
            throw new RegraDeNegocioException(
                $"O chamado {chamado.Protocolo} está encerrado e não aceita novas interações.");
        }

        StatusChamado novoStatus = dados.NovoStatus ?? chamado.Status;

        if (novoStatus != StatusChamado.Aberto && novoStatus != StatusChamado.EmAtendimento)
        {
            throw new RegraDeNegocioException(
                "Para encerrar o chamado utilize o endpoint de encerramento.");
        }

        var interacao = new ChamadoInteracao
        {
            Chamado = chamado,
            UsuarioId = _usuarioContexto.UsuarioId,
            Mensagem = dados.Mensagem.Trim(),
            StatusRegistrado = novoStatus,
            RegistradaEm = DateTime.UtcNow
        };

        chamado.Status = novoStatus;
        chamado.AtualizadoEm = DateTime.UtcNow;

        await _chamadoRepositorio.AdicionarInteracaoAsync(interacao, cancellationToken);
        await _chamadoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        ChamadoSuporte? atualizado = await _chamadoRepositorio.ObterCompletoAsync(id, cancellationToken);

        return ChamadoDto.DeModelo(atualizado ?? chamado);
    }

    public async Task<ChamadoDto> EncerrarAsync(
        int id,
        EncerrarChamadoDto dados,
        CancellationToken cancellationToken)
    {
        ChamadoSuporte chamado = await ObterChamadoAsync(id, cancellationToken);

        chamado.Encerrar(dados.SolucaoAplicada.Trim(), dados.StatusFinal);

        var interacao = new ChamadoInteracao
        {
            Chamado = chamado,
            UsuarioId = _usuarioContexto.UsuarioId,
            Mensagem = $"Chamado encerrado. Solução: {dados.SolucaoAplicada.Trim()}",
            StatusRegistrado = dados.StatusFinal,
            RegistradaEm = DateTime.UtcNow
        };

        await _chamadoRepositorio.AdicionarInteracaoAsync(interacao, cancellationToken);
        await _chamadoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        ChamadoSuporte? encerrado = await _chamadoRepositorio.ObterCompletoAsync(id, cancellationToken);

        return ChamadoDto.DeModelo(encerrado ?? chamado);
    }

    private async Task<ChamadoSuporte> ObterChamadoAsync(int id, CancellationToken cancellationToken)
    {
        ChamadoSuporte? chamado = await _chamadoRepositorio.ObterCompletoAsync(id, cancellationToken);

        if (chamado is null)
        {
            throw new NaoEncontradoException($"O chamado {id} não foi encontrado.");
        }

        _usuarioContexto.GarantirAcessoAUnidade(chamado.UnidadeFranqueadaId);

        return chamado;
    }
}
