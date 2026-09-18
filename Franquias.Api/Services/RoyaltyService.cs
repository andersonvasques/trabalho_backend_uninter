using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Security;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras de royalties.
///
/// O valor devido é sempre calculado como
/// faturamento confirmado do período x percentual da unidade.
/// </summary>
public sealed class RoyaltyService : IRoyaltyService
{
    private readonly IRoyaltyRepositorio _royaltyRepositorio;
    private readonly IUnidadeRepositorio _unidadeRepositorio;
    private readonly IVendaRepositorio _vendaRepositorio;
    private readonly IUsuarioContexto _usuarioContexto;

    public RoyaltyService(
        IRoyaltyRepositorio royaltyRepositorio,
        IUnidadeRepositorio unidadeRepositorio,
        IVendaRepositorio vendaRepositorio,
        IUsuarioContexto usuarioContexto)
    {
        _royaltyRepositorio = royaltyRepositorio;
        _unidadeRepositorio = unidadeRepositorio;
        _vendaRepositorio = vendaRepositorio;
        _usuarioContexto = usuarioContexto;
    }

    public async Task<ResultadoPaginado<RoyaltyDto>> ListarAsync(
        ConsultaRoyaltiesDto filtros,
        CancellationToken cancellationToken)
    {
        filtros.UnidadeFranqueadaId = _usuarioContexto.ResolverFiltroDeUnidade(filtros.UnidadeFranqueadaId);

        ResultadoPaginado<Royalty> pagina =
            await _royaltyRepositorio.ListarPaginadoAsync(filtros, cancellationToken);

        var itens = new List<RoyaltyDto>();

        foreach (Royalty royalty in pagina.Itens)
        {
            itens.Add(RoyaltyDto.DeModelo(royalty));
        }

        return new ResultadoPaginado<RoyaltyDto>(
            itens,
            pagina.TotalItens,
            pagina.Pagina,
            pagina.TamanhoPagina);
    }

    public async Task<RoyaltyDto> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        Royalty royalty = await ObterRoyaltyAsync(id, cancellationToken);

        return RoyaltyDto.DeModelo(royalty);
    }

    public async Task<IReadOnlyCollection<RoyaltyDto>> ApurarAsync(
        GerarRoyaltiesDto dados,
        CancellationToken cancellationToken)
    {
        DateTime inicioDoPeriodo = new DateTime(dados.Ano, dados.Mes, 1);
        DateTime fimDoPeriodo = inicioDoPeriodo.AddMonths(1).AddTicks(-1);
        DateTime vencimento = inicioDoPeriodo.AddMonths(1).AddDays(dados.DiaDoVencimento - 1);

        IReadOnlyCollection<UnidadeFranqueada> unidades =
            await ObterUnidadesDaApuracaoAsync(dados.UnidadeFranqueadaId, cancellationToken);

        if (unidades.Count == 0)
        {
            throw new RegraDeNegocioException(
                "Nenhuma unidade encontrada para a apuração dos royalties.");
        }

        var resultado = new List<Royalty>();

        foreach (UnidadeFranqueada unidade in unidades)
        {
            decimal faturamento = await _vendaRepositorio.CalcularFaturamentoAsync(
                unidade.Id,
                inicioDoPeriodo,
                fimDoPeriodo,
                cancellationToken);

            Royalty? royalty = await _royaltyRepositorio.ObterPorCompetenciaAsync(
                unidade.Id,
                dados.Ano,
                dados.Mes,
                cancellationToken);

            if (royalty is null)
            {
                // Apenas a chave estrangeira é informada: a entidade "unidade"
                // veio de uma consulta somente leitura e não deve ser reinserida.
                royalty = new Royalty
                {
                    UnidadeFranqueadaId = unidade.Id,
                    Ano = dados.Ano,
                    Mes = dados.Mes,
                    Situacao = SituacaoRoyalty.Pendente,
                    DataVencimento = vencimento,
                    GeradoEm = DateTime.UtcNow
                };

                royalty.Calcular(faturamento, unidade.PercentualRoyalty);

                await _royaltyRepositorio.AdicionarAsync(royalty, cancellationToken);
            }
            else
            {
                // Cobranças já pagas ou canceladas não são recalculadas.
                if (royalty.Situacao == SituacaoRoyalty.Pago ||
                    royalty.Situacao == SituacaoRoyalty.Cancelado)
                {
                    resultado.Add(royalty);
                    continue;
                }

                royalty.Calcular(faturamento, unidade.PercentualRoyalty);
                royalty.DataVencimento = vencimento;
            }

            AtualizarSituacaoPorVencimento(royalty);

            resultado.Add(royalty);
        }

        await _royaltyRepositorio.SalvarAlteracoesAsync(cancellationToken);

        var nomesDasUnidades = new Dictionary<int, string>();

        foreach (UnidadeFranqueada unidade in unidades)
        {
            nomesDasUnidades[unidade.Id] = unidade.NomeFantasia;
        }

        var dtos = new List<RoyaltyDto>();

        foreach (Royalty royalty in resultado)
        {
            RoyaltyDto dto = RoyaltyDto.DeModelo(royalty);

            if (string.IsNullOrEmpty(dto.UnidadeNome) &&
                nomesDasUnidades.TryGetValue(royalty.UnidadeFranqueadaId, out string? nome))
            {
                dto.UnidadeNome = nome;
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<RoyaltyDto> RegistrarPagamentoAsync(
        int id,
        RegistrarPagamentoRoyaltyDto dados,
        CancellationToken cancellationToken)
    {
        Royalty royalty = await ObterRoyaltyAsync(id, cancellationToken);

        royalty.RegistrarPagamento(
            decimal.Round(dados.ValorPago, 2),
            dados.DataPagamento ?? DateTime.UtcNow);

        await _royaltyRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return RoyaltyDto.DeModelo(royalty);
    }

    public async Task<RoyaltyDto> CancelarAsync(int id, CancellationToken cancellationToken)
    {
        Royalty royalty = await ObterRoyaltyAsync(id, cancellationToken);

        if (royalty.Situacao == SituacaoRoyalty.Pago)
        {
            throw new RegraDeNegocioException("Não é possível cancelar uma cobrança já paga.");
        }

        royalty.Situacao = SituacaoRoyalty.Cancelado;
        royalty.AtualizadoEm = DateTime.UtcNow;

        await _royaltyRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return RoyaltyDto.DeModelo(royalty);
    }

    private async Task<IReadOnlyCollection<UnidadeFranqueada>> ObterUnidadesDaApuracaoAsync(
        int? unidadeId,
        CancellationToken cancellationToken)
    {
        if (unidadeId.HasValue)
        {
            UnidadeFranqueada? unidade = await _unidadeRepositorio.ObterPorIdAsync(
                unidadeId.Value,
                cancellationToken);

            if (unidade is null)
            {
                throw new NaoEncontradoException($"A unidade {unidadeId} não foi encontrada.");
            }

            return new List<UnidadeFranqueada> { unidade };
        }

        return await _unidadeRepositorio.ListarAtivasAsync(cancellationToken);
    }

    /// <summary>
    /// Marca como atrasada a cobrança vencida que ainda não foi paga.
    /// </summary>
    private static void AtualizarSituacaoPorVencimento(Royalty royalty)
    {
        if (royalty.Situacao == SituacaoRoyalty.Pendente &&
            royalty.DataVencimento.Date < DateTime.UtcNow.Date &&
            royalty.ObterSaldoDevedor() > 0m)
        {
            royalty.Situacao = SituacaoRoyalty.Atrasado;
        }
    }

    private async Task<Royalty> ObterRoyaltyAsync(int id, CancellationToken cancellationToken)
    {
        Royalty? royalty = await _royaltyRepositorio.ObterComUnidadeAsync(id, cancellationToken);

        if (royalty is null)
        {
            throw new NaoEncontradoException($"A cobrança de royalty {id} não foi encontrada.");
        }

        _usuarioContexto.GarantirAcessoAUnidade(royalty.UnidadeFranqueadaId);

        return royalty;
    }
}
