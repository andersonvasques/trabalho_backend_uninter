using Franquias.Api.Models;
using Franquias.Api.Security;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

/// <summary>
/// Popula o banco de dados com uma massa de testes suficiente para
/// demonstrar cadastros, vendas, estoque, royalties, chamados e relatórios.
///
/// A carga só é executada quando o banco ainda não possui usuários.
/// </summary>
public static class SeedDados
{
    private const string SenhaPadrao = "Senha@123";

    public static async Task PopularAsync(AppDbContext contexto, CancellationToken cancellationToken = default)
    {
        bool jaPopulado = await contexto.Usuarios.AnyAsync(cancellationToken);

        if (jaPopulado)
        {
            return;
        }

        DateTime agora = DateTime.UtcNow;
        DateTime hoje = agora.Date;

        // =================================================================
        // 1. Franqueadora
        // =================================================================
        var franqueadora = new Franqueadora
        {
            RazaoSocial = "Sabor & Cia Franchising LTDA",
            NomeFantasia = "Sabor & Cia",
            Cnpj = "11223344000186",
            Email = "contato@saborecia.com.br",
            Telefone = "4133221100",
            Cidade = "Curitiba",
            Uf = "PR",
            PercentualRoyaltyPadrao = 5.00m,
            DataFundacao = new DateTime(2015, 3, 10),
            Ativa = true,
            CriadaEm = agora
        };

        contexto.Franqueadoras.Add(franqueadora);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 2. Franqueados
        // =================================================================
        var franqueados = new List<Franqueado>
        {
            NovoFranqueado("Ana Paula Ribeiro", "12345678909", "ana.ribeiro@saborecia.com.br", "41988887777", "Curitiba", "PR", agora),
            NovoFranqueado("Bruno Carvalho Lima", "23456789092", "bruno.lima@saborecia.com.br", "41977776666", "Londrina", "PR", agora),
            NovoFranqueado("Carla Mendes Souza", "34567890175", "carla.souza@saborecia.com.br", "11966665555", "São Paulo", "SP", agora),
            NovoFranqueado("Diego Almeida Rocha", "45678901249", "diego.rocha@saborecia.com.br", "51955554444", "Porto Alegre", "RS", agora)
        };

        contexto.Franqueados.AddRange(franqueados);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 3. Unidades franqueadas
        // =================================================================
        var unidades = new List<UnidadeFranqueada>
        {
            NovaUnidade("UN-001", franqueadora.Id, franqueados[0].Id, "Sabor Batel LTDA", "Sabor & Cia Batel",
                "22334455000186", "batel@saborecia.com.br", "Av. do Batel", "1230", "Batel", "Curitiba", "PR",
                "80420090", "Ana Paula Ribeiro", "41988887777", hoje.AddYears(-3), 5.00m, SituacaoUnidade.Ativa, agora),

            NovaUnidade("UN-002", franqueadora.Id, franqueados[1].Id, "Sabor Londrina LTDA", "Sabor & Cia Londrina",
                "33445566000186", "londrina@saborecia.com.br", "Rua Sergipe", "455", "Centro", "Londrina", "PR",
                "86010080", "Bruno Carvalho Lima", "41977776666", hoje.AddYears(-2), 5.00m, SituacaoUnidade.Ativa, agora),

            NovaUnidade("UN-003", franqueadora.Id, franqueados[2].Id, "Sabor Pinheiros LTDA", "Sabor & Cia Pinheiros",
                "44556677000186", "pinheiros@saborecia.com.br", "Rua dos Pinheiros", "780", "Pinheiros", "São Paulo", "SP",
                "05422001", "Carla Mendes Souza", "11966665555", hoje.AddYears(-1), 6.00m, SituacaoUnidade.Ativa, agora),

            NovaUnidade("UN-004", franqueadora.Id, franqueados[3].Id, "Sabor Moinhos LTDA", "Sabor & Cia Moinhos",
                "55667788000186", "moinhos@saborecia.com.br", "Rua Padre Chagas", "320", "Moinhos de Vento", "Porto Alegre", "RS",
                "90570080", "Diego Almeida Rocha", "51955554444", hoje.AddMonths(-8), 4.50m, SituacaoUnidade.Ativa, agora),

            NovaUnidade("UN-005", franqueadora.Id, franqueados[0].Id, "Sabor Centro LTDA", "Sabor & Cia Centro",
                "66778899000186", "centro@saborecia.com.br", "Rua XV de Novembro", "95", "Centro", "Curitiba", "PR",
                "80020310", "Ana Paula Ribeiro", "41988887777", hoje.AddYears(-4), 5.00m, SituacaoUnidade.Inativa, agora)
        };

        unidades[4].DataEncerramentoContrato = hoje.AddMonths(-6);

        contexto.Unidades.AddRange(unidades);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 4. Usuários (senha padrão: Senha@123)
        // =================================================================
        var usuarios = new List<Usuario>
        {
            NovoUsuario("Administrador da Rede", "admin@saborecia.com.br", PerfilUsuario.Administrador, null, agora),
            NovoUsuario("Ana Paula Ribeiro", "gestor.batel@saborecia.com.br", PerfilUsuario.Gestor, unidades[0].Id, agora),
            NovoUsuario("Bruno Carvalho Lima", "gestor.londrina@saborecia.com.br", PerfilUsuario.Gestor, unidades[1].Id, agora),
            NovoUsuario("Carla Mendes Souza", "gestor.pinheiros@saborecia.com.br", PerfilUsuario.Gestor, unidades[2].Id, agora),
            NovoUsuario("Diego Almeida Rocha", "gestor.moinhos@saborecia.com.br", PerfilUsuario.Gestor, unidades[3].Id, agora),
            NovoUsuario("Fernanda Dias", "operador.batel@saborecia.com.br", PerfilUsuario.Operador, unidades[0].Id, agora),
            NovoUsuario("Gustavo Nunes", "operador.londrina@saborecia.com.br", PerfilUsuario.Operador, unidades[1].Id, agora)
        };

        contexto.Usuarios.AddRange(usuarios);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 5. Categorias
        // =================================================================
        var categorias = new List<Categoria>
        {
            NovaCategoria("Bebidas", "Bebidas quentes e geladas.", agora),
            NovaCategoria("Alimentos", "Salgados, doces e lanches.", agora),
            NovaCategoria("Acessórios", "Itens de marca vendidos nas lojas.", agora),
            NovaCategoria("Serviços", "Serviços prestados pelas unidades.", agora)
        };

        contexto.Categorias.AddRange(categorias);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 6. Produtos e serviços
        // =================================================================
        var produtos = new List<ProdutoServico>
        {
            NovoProduto("BEB-001", "Café Expresso", "Café expresso tradicional 50 ml.", categorias[0].Id, TipoItem.Produto, 7.50m, 30, agora),
            NovoProduto("BEB-002", "Cappuccino", "Cappuccino cremoso 200 ml.", categorias[0].Id, TipoItem.Produto, 12.00m, 25, agora),
            NovoProduto("BEB-003", "Suco Natural de Laranja", "Suco natural 300 ml.", categorias[0].Id, TipoItem.Produto, 10.00m, 20, agora),
            NovoProduto("ALI-001", "Pão de Queijo", "Porção com 4 unidades.", categorias[1].Id, TipoItem.Produto, 9.00m, 40, agora),
            NovoProduto("ALI-002", "Sanduíche Natural", "Sanduíche de frango com salada.", categorias[1].Id, TipoItem.Produto, 18.90m, 15, agora),
            NovoProduto("ALI-003", "Bolo de Cenoura", "Fatia de bolo com cobertura.", categorias[1].Id, TipoItem.Produto, 11.50m, 15, agora),
            NovoProduto("ACE-001", "Caneca Sabor & Cia", "Caneca de porcelana 300 ml.", categorias[2].Id, TipoItem.Produto, 39.90m, 10, agora),
            NovoProduto("ACE-002", "Garrafa Térmica", "Garrafa térmica 500 ml da marca.", categorias[2].Id, TipoItem.Produto, 79.90m, 8, agora),
            NovoProduto("SER-001", "Coffee Break Empresarial", "Serviço de coffee break para eventos.", categorias[3].Id, TipoItem.Servico, 450.00m, 0, agora),
            NovoProduto("SER-002", "Assinatura Café do Mês", "Plano mensal de café da unidade.", categorias[3].Id, TipoItem.Servico, 89.90m, 0, agora)
        };

        contexto.ProdutosServicos.AddRange(produtos);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 7. Fornecedores e associação com produtos
        // =================================================================
        var fornecedores = new List<Fornecedor>
        {
            NovoFornecedor("Distribuidora Grão Nobre LTDA", "Grão Nobre", "77889900000166", "vendas@graonobre.com.br", "4132001000", "Curitiba", "PR", agora),
            NovoFornecedor("Panificadora Massa Fina LTDA", "Massa Fina", "88990011000107", "comercial@massafina.com.br", "4332002000", "Londrina", "PR", agora),
            NovoFornecedor("Brindes & Cia Comércio LTDA", "Brindes & Cia", "99001122000160", "contato@brindesecia.com.br", "1132003000", "São Paulo", "SP", agora)
        };

        contexto.Fornecedores.AddRange(fornecedores);
        await contexto.SaveChangesAsync(cancellationToken);

        var vinculos = new List<ProdutoFornecedor>
        {
            NovoVinculo(produtos[0].Id, fornecedores[0].Id, 2.10m, 3, true, agora),
            NovoVinculo(produtos[1].Id, fornecedores[0].Id, 4.30m, 3, true, agora),
            NovoVinculo(produtos[2].Id, fornecedores[0].Id, 3.80m, 2, false, agora),
            NovoVinculo(produtos[3].Id, fornecedores[1].Id, 3.20m, 1, true, agora),
            NovoVinculo(produtos[4].Id, fornecedores[1].Id, 7.90m, 1, true, agora),
            NovoVinculo(produtos[5].Id, fornecedores[1].Id, 4.50m, 2, false, agora),
            NovoVinculo(produtos[6].Id, fornecedores[2].Id, 18.00m, 10, true, agora),
            NovoVinculo(produtos[7].Id, fornecedores[2].Id, 41.00m, 12, true, agora)
        };

        contexto.ProdutosFornecedores.AddRange(vinculos);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 8. Estoque inicial das unidades ativas
        // =================================================================
        List<UnidadeFranqueada> unidadesAtivas = unidades
            .Where(unidade => unidade.Situacao == SituacaoUnidade.Ativa)
            .ToList();

        List<ProdutoServico> produtosFisicos = produtos
            .Where(produto => produto.Tipo == TipoItem.Produto)
            .ToList();

        var estoques = new List<EstoqueUnidade>();

        foreach (UnidadeFranqueada unidade in unidadesAtivas)
        {
            foreach (ProdutoServico produto in produtosFisicos)
            {
                var estoque = new EstoqueUnidade
                {
                    UnidadeFranqueadaId = unidade.Id,
                    ProdutoServicoId = produto.Id,
                    Quantidade = 250,
                    QuantidadeMinima = produto.EstoqueMinimoPadrao,
                    AtualizadoEm = agora
                };

                estoques.Add(estoque);
            }
        }

        contexto.EstoquesUnidades.AddRange(estoques);
        await contexto.SaveChangesAsync(cancellationToken);

        foreach (EstoqueUnidade estoque in estoques)
        {
            contexto.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                EstoqueUnidadeId = estoque.Id,
                Tipo = TipoMovimentacaoEstoque.Entrada,
                Quantidade = 250,
                SaldoAnterior = 0,
                SaldoAtual = 250,
                Motivo = "Carga inicial de estoque da unidade.",
                OcorridaEm = agora.AddMonths(-5)
            });
        }

        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 9. Vendas dos últimos 4 meses
        // =================================================================
        var gerador = new Random(20260916);
        var vendas = new List<Venda>();

        // A numeração é sequencial dentro de cada ano, no mesmo formato
        // utilizado pela API ao registrar novas vendas (VD-ano-000001).
        var sequenciaPorAno = new Dictionary<int, int>();

        for (int mesesAtras = 3; mesesAtras >= 0; mesesAtras--)
        {
            DateTime referencia = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-mesesAtras);

            foreach (UnidadeFranqueada unidade in unidadesAtivas)
            {
                Usuario usuarioDaVenda = usuarios.First(usuario =>
                    usuario.UnidadeFranqueadaId == unidade.Id);

                int[] dias = { 4, 11, 19, 26 };

                foreach (int dia in dias)
                {
                    DateTime dataVenda = referencia.AddDays(dia - 1).AddHours(10 + gerador.Next(0, 8));

                    if (dataVenda.Date > hoje)
                    {
                        continue;
                    }

                    sequenciaPorAno.TryGetValue(dataVenda.Year, out int sequencialVenda);
                    sequencialVenda++;
                    sequenciaPorAno[dataVenda.Year] = sequencialVenda;

                    var venda = new Venda
                    {
                        Numero = $"VD-{dataVenda.Year}-{sequencialVenda:D6}",
                        UnidadeFranqueadaId = unidade.Id,
                        UsuarioId = usuarioDaVenda.Id,
                        DataVenda = dataVenda,
                        Status = StatusVenda.Confirmada,
                        FormaPagamento = (FormaPagamento)gerador.Next(1, 6),
                        Desconto = 0m,
                        Cliente = "Cliente balcão",
                        Observacao = "Venda registrada na carga de exemplo.",
                        CriadaEm = dataVenda
                    };

                    int quantidadeDeItens = gerador.Next(1, 4);

                    var produtosSorteados = new List<ProdutoServico>();

                    while (produtosSorteados.Count < quantidadeDeItens)
                    {
                        ProdutoServico sorteado = produtos[gerador.Next(0, produtos.Count)];

                        if (!produtosSorteados.Contains(sorteado))
                        {
                            produtosSorteados.Add(sorteado);
                        }
                    }

                    foreach (ProdutoServico produto in produtosSorteados)
                    {
                        int quantidade = produto.Tipo == TipoItem.Servico ? 1 : gerador.Next(1, 6);

                        var item = new ItemVenda
                        {
                            ProdutoServicoId = produto.Id,
                            DescricaoItem = produto.Nome,
                            Quantidade = quantidade,
                            PrecoUnitario = produto.PrecoBase
                        };

                        item.CalcularSubtotal();
                        venda.Itens.Add(item);

                        if (produto.Tipo != TipoItem.Produto)
                        {
                            continue;
                        }

                        EstoqueUnidade estoque = estoques.First(saldo =>
                            saldo.UnidadeFranqueadaId == unidade.Id &&
                            saldo.ProdutoServicoId == produto.Id);

                        int saldoAnterior = estoque.Quantidade;

                        if (saldoAnterior < quantidade)
                        {
                            continue;
                        }

                        estoque.Quantidade = saldoAnterior - quantidade;
                        estoque.AtualizadoEm = dataVenda;

                        contexto.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                        {
                            EstoqueUnidadeId = estoque.Id,
                            Tipo = TipoMovimentacaoEstoque.Saida,
                            Quantidade = quantidade,
                            SaldoAnterior = saldoAnterior,
                            SaldoAtual = estoque.Quantidade,
                            Motivo = $"Baixa automática da venda {venda.Numero}.",
                            UsuarioId = usuarioDaVenda.Id,
                            OcorridaEm = dataVenda
                        });
                    }

                    venda.RecalcularTotal();
                    vendas.Add(venda);
                }
            }
        }

        contexto.Vendas.AddRange(vendas);
        await contexto.SaveChangesAsync(cancellationToken);

        // Um item de estoque propositalmente abaixo do mínimo,
        // para demonstrar o relatório de estoque crítico.
        EstoqueUnidade estoqueCritico = estoques.First(saldo =>
            saldo.UnidadeFranqueadaId == unidadesAtivas[0].Id &&
            saldo.ProdutoServicoId == produtos[4].Id);

        int saldoAntesDoAjuste = estoqueCritico.Quantidade;
        estoqueCritico.Quantidade = 5;
        estoqueCritico.AtualizadoEm = agora;

        contexto.MovimentacoesEstoque.Add(new MovimentacaoEstoque
        {
            EstoqueUnidadeId = estoqueCritico.Id,
            Tipo = TipoMovimentacaoEstoque.Ajuste,
            Quantidade = 5,
            SaldoAnterior = saldoAntesDoAjuste,
            SaldoAtual = 5,
            Motivo = "Ajuste de inventário.",
            OcorridaEm = agora.AddDays(-2)
        });

        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 10. Royalties dos três meses anteriores
        // =================================================================
        var royalties = new List<Royalty>();

        for (int mesesAtras = 3; mesesAtras >= 1; mesesAtras--)
        {
            DateTime competencia = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-mesesAtras);

            foreach (UnidadeFranqueada unidade in unidadesAtivas)
            {
                decimal faturamento = vendas
                    .Where(venda =>
                        venda.UnidadeFranqueadaId == unidade.Id &&
                        venda.Status == StatusVenda.Confirmada &&
                        venda.DataVenda.Year == competencia.Year &&
                        venda.DataVenda.Month == competencia.Month)
                    .Sum(venda => venda.ValorTotal);

                var royalty = new Royalty
                {
                    UnidadeFranqueadaId = unidade.Id,
                    Ano = competencia.Year,
                    Mes = competencia.Month,
                    DataVencimento = competencia.AddMonths(1).AddDays(9),
                    GeradoEm = competencia.AddMonths(1),
                    Situacao = SituacaoRoyalty.Pendente
                };

                royalty.Calcular(faturamento, unidade.PercentualRoyalty);

                // As competências mais antigas já aparecem quitadas.
                if (mesesAtras >= 2 && royalty.ValorDevido > 0m)
                {
                    royalty.ValorPago = royalty.ValorDevido;
                    royalty.Situacao = SituacaoRoyalty.Pago;
                    royalty.DataPagamento = royalty.DataVencimento.AddDays(-2);
                }

                royalties.Add(royalty);
            }
        }

        contexto.Royalties.AddRange(royalties);
        await contexto.SaveChangesAsync(cancellationToken);

        // =================================================================
        // 11. Chamados de suporte
        // =================================================================
        var chamados = new List<ChamadoSuporte>
        {
            NovoChamado(unidades[0].Id, usuarios[1].Id,
                "Erro ao emitir relatório de vendas",
                "O relatório de vendas do mês não está abrindo no sistema da loja.",
                CategoriaChamado.Sistema, PrioridadeChamado.Alta, StatusChamado.Aberto, agora.AddDays(-9)),

            NovoChamado(unidades[1].Id, usuarios[2].Id,
                "Atraso na entrega do fornecedor",
                "A entrega semanal da panificadora atrasou três dias e faltou produto na loja.",
                CategoriaChamado.Estoque, PrioridadeChamado.Critica, StatusChamado.EmAtendimento, agora.AddDays(-7)),

            NovoChamado(unidades[2].Id, usuarios[3].Id,
                "Dúvida sobre o cálculo do royalty",
                "Gostaria de entender como o percentual de 6% é aplicado sobre o faturamento.",
                CategoriaChamado.Financeiro, PrioridadeChamado.Media, StatusChamado.Resolvido, agora.AddDays(-20)),

            NovoChamado(unidades[3].Id, usuarios[4].Id,
                "Material de campanha de marketing",
                "Solicito o material gráfico da campanha de inverno para a unidade.",
                CategoriaChamado.Marketing, PrioridadeChamado.Baixa, StatusChamado.Aberto, agora.AddDays(-4)),

            NovoChamado(unidades[0].Id, usuarios[5].Id,
                "Treinamento de novo operador",
                "Precisamos de treinamento para o novo operador contratado pela unidade.",
                CategoriaChamado.Operacional, PrioridadeChamado.Media, StatusChamado.EmAtendimento, agora.AddDays(-2))
        };

        // Protocolos sequenciais por ano, no mesmo formato gerado pela API.
        var sequenciaDeProtocolo = new Dictionary<int, int>();

        foreach (ChamadoSuporte chamado in chamados.OrderBy(item => item.AbertoEm))
        {
            int ano = chamado.AbertoEm.Year;

            sequenciaDeProtocolo.TryGetValue(ano, out int sequencial);
            sequencial++;
            sequenciaDeProtocolo[ano] = sequencial;

            chamado.Protocolo = $"CH-{ano}-{sequencial:D6}";
        }

        chamados[2].SolucaoAplicada =
            "O royalty é calculado sobre o faturamento confirmado do mês, aplicando o percentual do contrato.";
        chamados[2].FechadoEm = agora.AddDays(-18);
        chamados[2].AtualizadoEm = agora.AddDays(-18);

        contexto.ChamadosSuporte.AddRange(chamados);
        await contexto.SaveChangesAsync(cancellationToken);

        var interacoes = new List<ChamadoInteracao>
        {
            NovaInteracao(chamados[0].Id, usuarios[1].Id, "Chamado aberto pela unidade.", StatusChamado.Aberto, chamados[0].AbertoEm),
            NovaInteracao(chamados[1].Id, usuarios[2].Id, "Chamado aberto pela unidade.", StatusChamado.Aberto, chamados[1].AbertoEm),
            NovaInteracao(chamados[1].Id, usuarios[0].Id, "Fornecedor acionado pela franqueadora.", StatusChamado.EmAtendimento, chamados[1].AbertoEm.AddDays(1)),
            NovaInteracao(chamados[2].Id, usuarios[3].Id, "Chamado aberto pela unidade.", StatusChamado.Aberto, chamados[2].AbertoEm),
            NovaInteracao(chamados[2].Id, usuarios[0].Id, "Cálculo explicado e chamado encerrado.", StatusChamado.Resolvido, agora.AddDays(-18)),
            NovaInteracao(chamados[3].Id, usuarios[4].Id, "Chamado aberto pela unidade.", StatusChamado.Aberto, chamados[3].AbertoEm),
            NovaInteracao(chamados[4].Id, usuarios[5].Id, "Chamado aberto pela unidade.", StatusChamado.Aberto, chamados[4].AbertoEm),
            NovaInteracao(chamados[4].Id, usuarios[0].Id, "Treinamento agendado para a próxima semana.", StatusChamado.EmAtendimento, agora.AddDays(-1))
        };

        contexto.ChamadosInteracoes.AddRange(interacoes);
        await contexto.SaveChangesAsync(cancellationToken);
    }

    // =====================================================================
    // Métodos de apoio
    // =====================================================================

    private static Franqueado NovoFranqueado(
        string nome, string cpf, string email, string telefone,
        string cidade, string uf, DateTime agora)
    {
        return new Franqueado
        {
            Nome = nome,
            Cpf = cpf,
            Email = email,
            Telefone = telefone,
            Cidade = cidade,
            Uf = uf,
            DataEntradaNaRede = agora.Date.AddYears(-3),
            Ativo = true,
            CriadoEm = agora
        };
    }

    private static UnidadeFranqueada NovaUnidade(
        string codigo, int franqueadoraId, int franqueadoId, string razaoSocial, string nomeFantasia,
        string cnpj, string email, string logradouro, string numero, string bairro, string cidade,
        string uf, string cep, string responsavel, string telefoneResponsavel,
        DateTime inicioContrato, decimal percentualRoyalty, SituacaoUnidade situacao, DateTime agora)
    {
        return new UnidadeFranqueada
        {
            Codigo = codigo,
            FranqueadoraId = franqueadoraId,
            FranqueadoId = franqueadoId,
            RazaoSocial = razaoSocial,
            NomeFantasia = nomeFantasia,
            Cnpj = cnpj,
            Email = email,
            Telefone = telefoneResponsavel,
            Logradouro = logradouro,
            Numero = numero,
            Bairro = bairro,
            Cidade = cidade,
            Uf = uf,
            Cep = cep,
            ResponsavelNome = responsavel,
            ResponsavelTelefone = telefoneResponsavel,
            DataInicioContrato = inicioContrato,
            PercentualRoyalty = percentualRoyalty,
            Situacao = situacao,
            CriadaEm = agora
        };
    }

    private static Usuario NovoUsuario(
        string nome, string email, PerfilUsuario perfil, int? unidadeId, DateTime agora)
    {
        (string hash, string salt) = SegurancaDeSenha.GerarHash(SenhaPadrao);

        return new Usuario
        {
            Nome = nome,
            Email = email,
            SenhaHash = hash,
            SenhaSalt = salt,
            Perfil = perfil,
            UnidadeFranqueadaId = unidadeId,
            Ativo = true,
            CriadoEm = agora
        };
    }

    private static Categoria NovaCategoria(string nome, string descricao, DateTime agora)
    {
        return new Categoria
        {
            Nome = nome,
            Descricao = descricao,
            Ativa = true,
            CriadaEm = agora
        };
    }

    private static ProdutoServico NovoProduto(
        string sku, string nome, string descricao, int categoriaId,
        TipoItem tipo, decimal preco, int estoqueMinimo, DateTime agora)
    {
        return new ProdutoServico
        {
            Sku = sku,
            Nome = nome,
            Descricao = descricao,
            CategoriaId = categoriaId,
            Tipo = tipo,
            PrecoBase = preco,
            EstoqueMinimoPadrao = estoqueMinimo,
            Ativo = true,
            CriadoEm = agora
        };
    }

    private static Fornecedor NovoFornecedor(
        string razaoSocial, string nomeFantasia, string cnpj, string email,
        string telefone, string cidade, string uf, DateTime agora)
    {
        return new Fornecedor
        {
            RazaoSocial = razaoSocial,
            NomeFantasia = nomeFantasia,
            Cnpj = cnpj,
            Email = email,
            Telefone = telefone,
            Cidade = cidade,
            Uf = uf,
            Ativo = true,
            CriadoEm = agora
        };
    }

    private static ProdutoFornecedor NovoVinculo(
        int produtoId, int fornecedorId, decimal precoCusto,
        int prazoEntrega, bool preferencial, DateTime agora)
    {
        return new ProdutoFornecedor
        {
            ProdutoServicoId = produtoId,
            FornecedorId = fornecedorId,
            PrecoCusto = precoCusto,
            PrazoEntregaDias = prazoEntrega,
            Preferencial = preferencial,
            CriadaEm = agora
        };
    }

    private static ChamadoSuporte NovoChamado(
        int unidadeId, int usuarioId, string titulo, string descricao,
        CategoriaChamado categoria, PrioridadeChamado prioridade, StatusChamado status, DateTime abertoEm)
    {
        return new ChamadoSuporte
        {
            Protocolo = string.Empty,
            UnidadeFranqueadaId = unidadeId,
            UsuarioAberturaId = usuarioId,
            Titulo = titulo,
            Descricao = descricao,
            Categoria = categoria,
            Prioridade = prioridade,
            Status = status,
            AbertoEm = abertoEm
        };
    }

    private static ChamadoInteracao NovaInteracao(
        int chamadoId, int usuarioId, string mensagem, StatusChamado status, DateTime registradaEm)
    {
        return new ChamadoInteracao
        {
            ChamadoSuporteId = chamadoId,
            UsuarioId = usuarioId,
            Mensagem = mensagem,
            StatusRegistrado = status,
            RegistradaEm = registradaEm
        };
    }
}
