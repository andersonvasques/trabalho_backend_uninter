using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Validations;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação das regras de cadastro da franqueadora e dos franqueados.
/// Utiliza o repositório genérico, já que as operações são CRUDs simples.
/// </summary>
public sealed class RedeService : IRedeService
{
    private readonly IRepositorioBase<Franqueadora> _franqueadoraRepositorio;
    private readonly IRepositorioBase<Franqueado> _franqueadoRepositorio;
    private readonly IUnidadeRepositorio _unidadeRepositorio;

    public RedeService(
        IRepositorioBase<Franqueadora> franqueadoraRepositorio,
        IRepositorioBase<Franqueado> franqueadoRepositorio,
        IUnidadeRepositorio unidadeRepositorio)
    {
        _franqueadoraRepositorio = franqueadoraRepositorio;
        _franqueadoRepositorio = franqueadoRepositorio;
        _unidadeRepositorio = unidadeRepositorio;
    }

    // =====================================================================
    // Franqueadora
    // =====================================================================

    public async Task<IReadOnlyCollection<FranqueadoraDto>> ListarFranqueadorasAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Franqueadora> franqueadoras =
            await _franqueadoraRepositorio.ListarAsync(null, cancellationToken);

        IReadOnlyCollection<UnidadeFranqueada> unidades =
            await _unidadeRepositorio.ListarAsync(null, cancellationToken);

        var resultado = new List<FranqueadoraDto>();

        foreach (Franqueadora franqueadora in franqueadoras)
        {
            int quantidade = ContarUnidades(unidades, unidade => unidade.FranqueadoraId == franqueadora.Id);
            resultado.Add(FranqueadoraDto.DeModelo(franqueadora, quantidade));
        }

        return resultado;
    }

    public async Task<FranqueadoraDto> ObterFranqueadoraAsync(int id, CancellationToken cancellationToken)
    {
        Franqueadora franqueadora = await ObterFranqueadoraOuFalharAsync(id, cancellationToken);

        IReadOnlyCollection<UnidadeFranqueada> unidades =
            await _unidadeRepositorio.ListarAsync(
                unidade => unidade.FranqueadoraId == id,
                cancellationToken);

        return FranqueadoraDto.DeModelo(franqueadora, unidades.Count);
    }

    public async Task<FranqueadoraDto> CriarFranqueadoraAsync(
        FranqueadoraEntradaDto dados,
        CancellationToken cancellationToken)
    {
        string cnpj = ValidadorDeDocumentos.SomenteDigitos(dados.Cnpj);

        bool cnpjEmUso = await _franqueadoraRepositorio.ExisteAsync(
            franqueadora => franqueadora.Cnpj == cnpj,
            cancellationToken);

        if (cnpjEmUso)
        {
            throw new ConflitoException($"Já existe uma franqueadora cadastrada com o CNPJ {cnpj}.");
        }

        var franqueadora = new Franqueadora
        {
            RazaoSocial = dados.RazaoSocial.Trim(),
            NomeFantasia = dados.NomeFantasia.Trim(),
            Cnpj = cnpj,
            Email = dados.Email.Trim().ToLower(),
            Telefone = dados.Telefone.Trim(),
            Cidade = dados.Cidade.Trim(),
            Uf = dados.Uf.Trim().ToUpper(),
            PercentualRoyaltyPadrao = dados.PercentualRoyaltyPadrao,
            DataFundacao = dados.DataFundacao,
            Ativa = true,
            CriadaEm = DateTime.UtcNow
        };

        await _franqueadoraRepositorio.AdicionarAsync(franqueadora, cancellationToken);
        await _franqueadoraRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoraDto.DeModelo(franqueadora);
    }

    public async Task<FranqueadoraDto> AtualizarFranqueadoraAsync(
        int id,
        FranqueadoraEntradaDto dados,
        CancellationToken cancellationToken)
    {
        Franqueadora franqueadora = await ObterFranqueadoraOuFalharAsync(id, cancellationToken);

        string cnpj = ValidadorDeDocumentos.SomenteDigitos(dados.Cnpj);

        bool cnpjEmUso = await _franqueadoraRepositorio.ExisteAsync(
            outra => outra.Cnpj == cnpj && outra.Id != id,
            cancellationToken);

        if (cnpjEmUso)
        {
            throw new ConflitoException($"Já existe outra franqueadora cadastrada com o CNPJ {cnpj}.");
        }

        franqueadora.RazaoSocial = dados.RazaoSocial.Trim();
        franqueadora.NomeFantasia = dados.NomeFantasia.Trim();
        franqueadora.Cnpj = cnpj;
        franqueadora.Email = dados.Email.Trim().ToLower();
        franqueadora.Telefone = dados.Telefone.Trim();
        franqueadora.Cidade = dados.Cidade.Trim();
        franqueadora.Uf = dados.Uf.Trim().ToUpper();
        franqueadora.PercentualRoyaltyPadrao = dados.PercentualRoyaltyPadrao;
        franqueadora.DataFundacao = dados.DataFundacao;
        franqueadora.AtualizadaEm = DateTime.UtcNow;

        await _franqueadoraRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoraDto.DeModelo(franqueadora);
    }

    // =====================================================================
    // Franqueados
    // =====================================================================

    public async Task<ResultadoPaginado<FranqueadoDto>> ListarFranqueadosAsync(
        ConsultaFranqueadosDto filtros,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Franqueado> franqueados =
            await _franqueadoRepositorio.ListarAsync(null, cancellationToken);

        IReadOnlyCollection<UnidadeFranqueada> unidades =
            await _unidadeRepositorio.ListarAsync(null, cancellationToken);

        IEnumerable<Franqueado> consulta = franqueados;

        if (!string.IsNullOrWhiteSpace(filtros.Busca))
        {
            string termo = filtros.Busca.Trim().ToLower();

            consulta = consulta.Where(franqueado =>
                franqueado.Nome.ToLower().Contains(termo) ||
                franqueado.Cpf.Contains(termo) ||
                franqueado.Email.ToLower().Contains(termo));
        }

        if (filtros.Ativo.HasValue)
        {
            consulta = consulta.Where(franqueado => franqueado.Ativo == filtros.Ativo.Value);
        }

        consulta = filtros.Decrescente
            ? consulta.OrderByDescending(franqueado => franqueado.Nome)
            : consulta.OrderBy(franqueado => franqueado.Nome);

        List<Franqueado> lista = consulta.ToList();

        List<FranqueadoDto> pagina = lista
            .Skip(filtros.RegistrosParaPular())
            .Take(filtros.TamanhoPagina)
            .Select(franqueado => FranqueadoDto.DeModelo(
                franqueado,
                ContarUnidades(unidades, unidade => unidade.FranqueadoId == franqueado.Id)))
            .ToList();

        return new ResultadoPaginado<FranqueadoDto>(
            pagina,
            lista.Count,
            filtros.Pagina,
            filtros.TamanhoPagina);
    }

    public async Task<FranqueadoDto> ObterFranqueadoAsync(int id, CancellationToken cancellationToken)
    {
        Franqueado franqueado = await ObterFranqueadoOuFalharAsync(id, cancellationToken);

        IReadOnlyCollection<UnidadeFranqueada> unidades =
            await _unidadeRepositorio.ListarAsync(
                unidade => unidade.FranqueadoId == id,
                cancellationToken);

        return FranqueadoDto.DeModelo(franqueado, unidades.Count);
    }

    public async Task<FranqueadoDto> CriarFranqueadoAsync(
        FranqueadoEntradaDto dados,
        CancellationToken cancellationToken)
    {
        string cpf = ValidadorDeDocumentos.SomenteDigitos(dados.Cpf);

        bool cpfEmUso = await _franqueadoRepositorio.ExisteAsync(
            franqueado => franqueado.Cpf == cpf,
            cancellationToken);

        if (cpfEmUso)
        {
            throw new ConflitoException($"Já existe um franqueado cadastrado com o CPF {cpf}.");
        }

        var novo = new Franqueado
        {
            Nome = dados.Nome.Trim(),
            Cpf = cpf,
            Email = dados.Email.Trim().ToLower(),
            Telefone = dados.Telefone.Trim(),
            Cidade = dados.Cidade.Trim(),
            Uf = dados.Uf.Trim().ToUpper(),
            DataEntradaNaRede = dados.DataEntradaNaRede == default
                ? DateTime.UtcNow.Date
                : dados.DataEntradaNaRede,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        await _franqueadoRepositorio.AdicionarAsync(novo, cancellationToken);
        await _franqueadoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoDto.DeModelo(novo);
    }

    public async Task<FranqueadoDto> AtualizarFranqueadoAsync(
        int id,
        FranqueadoEntradaDto dados,
        CancellationToken cancellationToken)
    {
        Franqueado franqueado = await ObterFranqueadoOuFalharAsync(id, cancellationToken);

        string cpf = ValidadorDeDocumentos.SomenteDigitos(dados.Cpf);

        bool cpfEmUso = await _franqueadoRepositorio.ExisteAsync(
            outro => outro.Cpf == cpf && outro.Id != id,
            cancellationToken);

        if (cpfEmUso)
        {
            throw new ConflitoException($"Já existe outro franqueado cadastrado com o CPF {cpf}.");
        }

        franqueado.Nome = dados.Nome.Trim();
        franqueado.Cpf = cpf;
        franqueado.Email = dados.Email.Trim().ToLower();
        franqueado.Telefone = dados.Telefone.Trim();
        franqueado.Cidade = dados.Cidade.Trim();
        franqueado.Uf = dados.Uf.Trim().ToUpper();

        if (dados.DataEntradaNaRede != default)
        {
            franqueado.DataEntradaNaRede = dados.DataEntradaNaRede;
        }

        franqueado.AtualizadoEm = DateTime.UtcNow;

        await _franqueadoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoDto.DeModelo(franqueado);
    }

    public async Task<FranqueadoDto> AlterarSituacaoFranqueadoAsync(
        int id,
        bool ativo,
        CancellationToken cancellationToken)
    {
        Franqueado franqueado = await ObterFranqueadoOuFalharAsync(id, cancellationToken);

        if (!ativo)
        {
            // Regra de negócio: um franqueado com unidades ativas não pode ser inativado.
            IReadOnlyCollection<UnidadeFranqueada> unidadesAtivas =
                await _unidadeRepositorio.ListarAsync(
                    unidade => unidade.FranqueadoId == id &&
                               unidade.Situacao == SituacaoUnidade.Ativa,
                    cancellationToken);

            if (unidadesAtivas.Count > 0)
            {
                throw new RegraDeNegocioException(
                    "Não é possível inativar um franqueado que ainda possui unidades ativas.");
            }
        }

        franqueado.Ativo = ativo;
        franqueado.AtualizadoEm = DateTime.UtcNow;

        await _franqueadoRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoDto.DeModelo(franqueado);
    }

    // =====================================================================
    // Apoio
    // =====================================================================

    private async Task<Franqueadora> ObterFranqueadoraOuFalharAsync(
        int id,
        CancellationToken cancellationToken)
    {
        Franqueadora? franqueadora = await _franqueadoraRepositorio.ObterPorIdAsync(id, cancellationToken);

        if (franqueadora is null)
        {
            throw new NaoEncontradoException($"A franqueadora {id} não foi encontrada.");
        }

        return franqueadora;
    }

    private async Task<Franqueado> ObterFranqueadoOuFalharAsync(
        int id,
        CancellationToken cancellationToken)
    {
        Franqueado? franqueado = await _franqueadoRepositorio.ObterPorIdAsync(id, cancellationToken);

        if (franqueado is null)
        {
            throw new NaoEncontradoException($"O franqueado {id} não foi encontrado.");
        }

        return franqueado;
    }

    private static int ContarUnidades(
        IReadOnlyCollection<UnidadeFranqueada> unidades,
        Func<UnidadeFranqueada, bool> filtro)
    {
        int quantidade = 0;

        foreach (UnidadeFranqueada unidade in unidades)
        {
            if (filtro(unidade))
            {
                quantidade++;
            }
        }

        return quantidade;
    }
}
