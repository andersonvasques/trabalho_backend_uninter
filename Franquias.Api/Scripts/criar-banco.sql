-- =====================================================================
-- Sistema de Gestão de Franquias
-- Script de criação do banco de dados (SQLite)
--
-- Este script é equivalente ao esquema gerado pelo Entity Framework Core.
-- A aplicação cria o banco automaticamente ao iniciar; o script é
-- disponibilizado para consulta, documentação e correção do trabalho.
-- =====================================================================

PRAGMA foreign_keys = ON;

-- ---------------------------------------------------------------------
-- Franqueadora (matriz da rede)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Franqueadoras" (
    "Id"                      INTEGER NOT NULL CONSTRAINT "PK_Franqueadoras" PRIMARY KEY AUTOINCREMENT,
    "RazaoSocial"             TEXT    NOT NULL,
    "NomeFantasia"            TEXT    NOT NULL,
    "Cnpj"                    TEXT    NOT NULL,
    "Email"                   TEXT    NOT NULL,
    "Telefone"                TEXT    NOT NULL,
    "Cidade"                  TEXT    NOT NULL,
    "Uf"                      TEXT    NOT NULL,
    "PercentualRoyaltyPadrao" REAL    NOT NULL,
    "DataFundacao"            TEXT    NOT NULL,
    "Ativa"                   INTEGER NOT NULL,
    "CriadaEm"                TEXT    NOT NULL,
    "AtualizadaEm"            TEXT    NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Franqueadoras_Cnpj" ON "Franqueadoras" ("Cnpj");

-- ---------------------------------------------------------------------
-- Franqueados (responsáveis pelas unidades)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Franqueados" (
    "Id"                 INTEGER NOT NULL CONSTRAINT "PK_Franqueados" PRIMARY KEY AUTOINCREMENT,
    "Nome"               TEXT    NOT NULL,
    "Cpf"                TEXT    NOT NULL,
    "Email"              TEXT    NOT NULL,
    "Telefone"           TEXT    NOT NULL,
    "Cidade"             TEXT    NOT NULL,
    "Uf"                 TEXT    NOT NULL,
    "DataEntradaNaRede"  TEXT    NOT NULL,
    "Ativo"              INTEGER NOT NULL,
    "CriadoEm"           TEXT    NOT NULL,
    "AtualizadoEm"       TEXT    NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Franqueados_Cpf" ON "Franqueados" ("Cpf");

-- ---------------------------------------------------------------------
-- Unidades franqueadas
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "UnidadesFranqueadas" (
    "Id"                       INTEGER NOT NULL CONSTRAINT "PK_UnidadesFranqueadas" PRIMARY KEY AUTOINCREMENT,
    "Codigo"                   TEXT    NOT NULL,
    "FranqueadoraId"           INTEGER NOT NULL,
    "FranqueadoId"             INTEGER NOT NULL,
    "RazaoSocial"              TEXT    NOT NULL,
    "NomeFantasia"             TEXT    NOT NULL,
    "Cnpj"                     TEXT    NOT NULL,
    "Email"                    TEXT    NOT NULL,
    "Telefone"                 TEXT    NOT NULL,
    "Logradouro"               TEXT    NOT NULL,
    "Numero"                   TEXT    NOT NULL,
    "Bairro"                   TEXT    NOT NULL,
    "Cidade"                   TEXT    NOT NULL,
    "Uf"                       TEXT    NOT NULL,
    "Cep"                      TEXT    NOT NULL,
    "ResponsavelNome"          TEXT    NOT NULL,
    "ResponsavelTelefone"      TEXT    NOT NULL,
    "DataInicioContrato"       TEXT    NOT NULL,
    "DataEncerramentoContrato" TEXT    NULL,
    "PercentualRoyalty"        REAL    NOT NULL,
    "Situacao"                 TEXT    NOT NULL,
    "CriadaEm"                 TEXT    NOT NULL,
    "AtualizadaEm"             TEXT    NULL,
    CONSTRAINT "FK_UnidadesFranqueadas_Franqueadoras" FOREIGN KEY ("FranqueadoraId")
        REFERENCES "Franqueadoras" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_UnidadesFranqueadas_Franqueados" FOREIGN KEY ("FranqueadoId")
        REFERENCES "Franqueados" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_UnidadesFranqueadas_Cnpj"   ON "UnidadesFranqueadas" ("Cnpj");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_UnidadesFranqueadas_Codigo" ON "UnidadesFranqueadas" ("Codigo");
CREATE INDEX        IF NOT EXISTS "IX_UnidadesFranqueadas_Situacao" ON "UnidadesFranqueadas" ("Situacao");
CREATE INDEX        IF NOT EXISTS "IX_UnidadesFranqueadas_Cidade"   ON "UnidadesFranqueadas" ("Cidade");

-- ---------------------------------------------------------------------
-- Usuários do sistema
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Usuarios" (
    "Id"                   INTEGER NOT NULL CONSTRAINT "PK_Usuarios" PRIMARY KEY AUTOINCREMENT,
    "Nome"                 TEXT    NOT NULL,
    "Email"                TEXT    NOT NULL,
    "SenhaHash"            TEXT    NOT NULL,
    "SenhaSalt"            TEXT    NOT NULL,
    "Perfil"               TEXT    NOT NULL,
    "UnidadeFranqueadaId"  INTEGER NULL,
    "Ativo"                INTEGER NOT NULL,
    "CriadoEm"             TEXT    NOT NULL,
    "AtualizadoEm"         TEXT    NULL,
    "UltimoAcessoEm"       TEXT    NULL,
    CONSTRAINT "FK_Usuarios_UnidadesFranqueadas" FOREIGN KEY ("UnidadeFranqueadaId")
        REFERENCES "UnidadesFranqueadas" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Usuarios_Email" ON "Usuarios" ("Email");

-- ---------------------------------------------------------------------
-- Categorias do catálogo
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Categorias" (
    "Id"           INTEGER NOT NULL CONSTRAINT "PK_Categorias" PRIMARY KEY AUTOINCREMENT,
    "Nome"         TEXT    NOT NULL,
    "Descricao"    TEXT    NOT NULL,
    "Ativa"        INTEGER NOT NULL,
    "CriadaEm"     TEXT    NOT NULL,
    "AtualizadaEm" TEXT    NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Categorias_Nome" ON "Categorias" ("Nome");

-- ---------------------------------------------------------------------
-- Produtos e serviços
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "ProdutosServicos" (
    "Id"                  INTEGER NOT NULL CONSTRAINT "PK_ProdutosServicos" PRIMARY KEY AUTOINCREMENT,
    "Sku"                 TEXT    NOT NULL,
    "Nome"                TEXT    NOT NULL,
    "Descricao"           TEXT    NOT NULL,
    "CategoriaId"         INTEGER NOT NULL,
    "Tipo"                TEXT    NOT NULL,
    "PrecoBase"           REAL    NOT NULL,
    "EstoqueMinimoPadrao" INTEGER NOT NULL,
    "Ativo"               INTEGER NOT NULL,
    "CriadoEm"            TEXT    NOT NULL,
    "AtualizadoEm"        TEXT    NULL,
    CONSTRAINT "FK_ProdutosServicos_Categorias" FOREIGN KEY ("CategoriaId")
        REFERENCES "Categorias" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_ProdutosServicos_Sku"  ON "ProdutosServicos" ("Sku");
CREATE INDEX        IF NOT EXISTS "IX_ProdutosServicos_Nome" ON "ProdutosServicos" ("Nome");

-- ---------------------------------------------------------------------
-- Fornecedores
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Fornecedores" (
    "Id"           INTEGER NOT NULL CONSTRAINT "PK_Fornecedores" PRIMARY KEY AUTOINCREMENT,
    "RazaoSocial"  TEXT    NOT NULL,
    "NomeFantasia" TEXT    NOT NULL,
    "Cnpj"         TEXT    NOT NULL,
    "Email"        TEXT    NOT NULL,
    "Telefone"     TEXT    NOT NULL,
    "Cidade"       TEXT    NOT NULL,
    "Uf"           TEXT    NOT NULL,
    "Ativo"        INTEGER NOT NULL,
    "CriadoEm"     TEXT    NOT NULL,
    "AtualizadoEm" TEXT    NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Fornecedores_Cnpj"        ON "Fornecedores" ("Cnpj");
CREATE INDEX        IF NOT EXISTS "IX_Fornecedores_RazaoSocial" ON "Fornecedores" ("RazaoSocial");

-- ---------------------------------------------------------------------
-- Associação produto x fornecedor (N:N com dados adicionais)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "ProdutosFornecedores" (
    "ProdutoServicoId" INTEGER NOT NULL,
    "FornecedorId"     INTEGER NOT NULL,
    "PrecoCusto"       REAL    NOT NULL,
    "PrazoEntregaDias" INTEGER NOT NULL,
    "Preferencial"     INTEGER NOT NULL,
    "CriadaEm"         TEXT    NOT NULL,
    CONSTRAINT "PK_ProdutosFornecedores" PRIMARY KEY ("ProdutoServicoId", "FornecedorId"),
    CONSTRAINT "FK_ProdutosFornecedores_ProdutosServicos" FOREIGN KEY ("ProdutoServicoId")
        REFERENCES "ProdutosServicos" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProdutosFornecedores_Fornecedores" FOREIGN KEY ("FornecedorId")
        REFERENCES "Fornecedores" ("Id") ON DELETE CASCADE
);

-- ---------------------------------------------------------------------
-- Estoque por unidade
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "EstoquesUnidades" (
    "Id"                  INTEGER NOT NULL CONSTRAINT "PK_EstoquesUnidades" PRIMARY KEY AUTOINCREMENT,
    "UnidadeFranqueadaId" INTEGER NOT NULL,
    "ProdutoServicoId"    INTEGER NOT NULL,
    "Quantidade"          INTEGER NOT NULL,
    "QuantidadeMinima"    INTEGER NOT NULL,
    "AtualizadoEm"        TEXT    NOT NULL,
    CONSTRAINT "FK_EstoquesUnidades_UnidadesFranqueadas" FOREIGN KEY ("UnidadeFranqueadaId")
        REFERENCES "UnidadesFranqueadas" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EstoquesUnidades_ProdutosServicos" FOREIGN KEY ("ProdutoServicoId")
        REFERENCES "ProdutosServicos" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_EstoquesUnidades_Unidade_Produto"
    ON "EstoquesUnidades" ("UnidadeFranqueadaId", "ProdutoServicoId");

-- ---------------------------------------------------------------------
-- Movimentações de estoque
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "MovimentacoesEstoque" (
    "Id"                INTEGER NOT NULL CONSTRAINT "PK_MovimentacoesEstoque" PRIMARY KEY AUTOINCREMENT,
    "EstoqueUnidadeId"  INTEGER NOT NULL,
    "Tipo"              TEXT    NOT NULL,
    "Quantidade"        INTEGER NOT NULL,
    "SaldoAnterior"     INTEGER NOT NULL,
    "SaldoAtual"        INTEGER NOT NULL,
    "Motivo"            TEXT    NOT NULL,
    "VendaId"           INTEGER NULL,
    "UsuarioId"         INTEGER NULL,
    "OcorridaEm"        TEXT    NOT NULL,
    CONSTRAINT "FK_MovimentacoesEstoque_EstoquesUnidades" FOREIGN KEY ("EstoqueUnidadeId")
        REFERENCES "EstoquesUnidades" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MovimentacoesEstoque_Vendas" FOREIGN KEY ("VendaId")
        REFERENCES "Vendas" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_MovimentacoesEstoque_Usuarios" FOREIGN KEY ("UsuarioId")
        REFERENCES "Usuarios" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_MovimentacoesEstoque_OcorridaEm" ON "MovimentacoesEstoque" ("OcorridaEm");

-- ---------------------------------------------------------------------
-- Vendas
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Vendas" (
    "Id"                  INTEGER NOT NULL CONSTRAINT "PK_Vendas" PRIMARY KEY AUTOINCREMENT,
    "Numero"              TEXT    NOT NULL,
    "UnidadeFranqueadaId" INTEGER NOT NULL,
    "UsuarioId"           INTEGER NOT NULL,
    "DataVenda"           TEXT    NOT NULL,
    "Status"              TEXT    NOT NULL,
    "FormaPagamento"      TEXT    NOT NULL,
    "Desconto"            REAL    NOT NULL,
    "ValorTotal"          REAL    NOT NULL,
    "Cliente"             TEXT    NULL,
    "Observacao"          TEXT    NULL,
    "CriadaEm"            TEXT    NOT NULL,
    "CanceladaEm"         TEXT    NULL,
    "MotivoCancelamento"  TEXT    NULL,
    CONSTRAINT "FK_Vendas_UnidadesFranqueadas" FOREIGN KEY ("UnidadeFranqueadaId")
        REFERENCES "UnidadesFranqueadas" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Vendas_Usuarios" FOREIGN KEY ("UsuarioId")
        REFERENCES "Usuarios" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Vendas_Numero"            ON "Vendas" ("Numero");
CREATE INDEX        IF NOT EXISTS "IX_Vendas_DataVenda"         ON "Vendas" ("DataVenda");
CREATE INDEX        IF NOT EXISTS "IX_Vendas_Unidade_DataVenda" ON "Vendas" ("UnidadeFranqueadaId", "DataVenda");

-- ---------------------------------------------------------------------
-- Itens da venda
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "ItensVenda" (
    "Id"               INTEGER NOT NULL CONSTRAINT "PK_ItensVenda" PRIMARY KEY AUTOINCREMENT,
    "VendaId"          INTEGER NOT NULL,
    "ProdutoServicoId" INTEGER NOT NULL,
    "DescricaoItem"    TEXT    NOT NULL,
    "Quantidade"       INTEGER NOT NULL,
    "PrecoUnitario"    REAL    NOT NULL,
    "Subtotal"         REAL    NOT NULL,
    CONSTRAINT "FK_ItensVenda_Vendas" FOREIGN KEY ("VendaId")
        REFERENCES "Vendas" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ItensVenda_ProdutosServicos" FOREIGN KEY ("ProdutoServicoId")
        REFERENCES "ProdutosServicos" ("Id") ON DELETE RESTRICT
);

-- ---------------------------------------------------------------------
-- Royalties
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Royalties" (
    "Id"                  INTEGER NOT NULL CONSTRAINT "PK_Royalties" PRIMARY KEY AUTOINCREMENT,
    "UnidadeFranqueadaId" INTEGER NOT NULL,
    "Ano"                 INTEGER NOT NULL,
    "Mes"                 INTEGER NOT NULL,
    "FaturamentoBase"     REAL    NOT NULL,
    "PercentualAplicado"  REAL    NOT NULL,
    "ValorDevido"         REAL    NOT NULL,
    "ValorPago"           REAL    NOT NULL,
    "Situacao"            TEXT    NOT NULL,
    "DataVencimento"      TEXT    NOT NULL,
    "DataPagamento"       TEXT    NULL,
    "GeradoEm"            TEXT    NOT NULL,
    "AtualizadoEm"        TEXT    NULL,
    CONSTRAINT "FK_Royalties_UnidadesFranqueadas" FOREIGN KEY ("UnidadeFranqueadaId")
        REFERENCES "UnidadesFranqueadas" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Royalties_Unidade_Competencia"
    ON "Royalties" ("UnidadeFranqueadaId", "Ano", "Mes");

CREATE INDEX IF NOT EXISTS "IX_Royalties_Situacao" ON "Royalties" ("Situacao");

-- ---------------------------------------------------------------------
-- Chamados de suporte
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "ChamadosSuporte" (
    "Id"                  INTEGER NOT NULL CONSTRAINT "PK_ChamadosSuporte" PRIMARY KEY AUTOINCREMENT,
    "Protocolo"           TEXT    NOT NULL,
    "UnidadeFranqueadaId" INTEGER NOT NULL,
    "UsuarioAberturaId"   INTEGER NOT NULL,
    "Titulo"              TEXT    NOT NULL,
    "Descricao"           TEXT    NOT NULL,
    "Categoria"           TEXT    NOT NULL,
    "Prioridade"          TEXT    NOT NULL,
    "Status"              TEXT    NOT NULL,
    "AbertoEm"            TEXT    NOT NULL,
    "AtualizadoEm"        TEXT    NULL,
    "FechadoEm"           TEXT    NULL,
    "SolucaoAplicada"     TEXT    NULL,
    CONSTRAINT "FK_ChamadosSuporte_UnidadesFranqueadas" FOREIGN KEY ("UnidadeFranqueadaId")
        REFERENCES "UnidadesFranqueadas" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ChamadosSuporte_Usuarios" FOREIGN KEY ("UsuarioAberturaId")
        REFERENCES "Usuarios" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_ChamadosSuporte_Protocolo"  ON "ChamadosSuporte" ("Protocolo");
CREATE INDEX        IF NOT EXISTS "IX_ChamadosSuporte_Status"     ON "ChamadosSuporte" ("Status");
CREATE INDEX        IF NOT EXISTS "IX_ChamadosSuporte_Prioridade" ON "ChamadosSuporte" ("Prioridade");

-- ---------------------------------------------------------------------
-- Interações dos chamados
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "ChamadosInteracoes" (
    "Id"               INTEGER NOT NULL CONSTRAINT "PK_ChamadosInteracoes" PRIMARY KEY AUTOINCREMENT,
    "ChamadoSuporteId" INTEGER NOT NULL,
    "UsuarioId"        INTEGER NOT NULL,
    "Mensagem"         TEXT    NOT NULL,
    "StatusRegistrado" TEXT    NOT NULL,
    "RegistradaEm"     TEXT    NOT NULL,
    CONSTRAINT "FK_ChamadosInteracoes_ChamadosSuporte" FOREIGN KEY ("ChamadoSuporteId")
        REFERENCES "ChamadosSuporte" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ChamadosInteracoes_Usuarios" FOREIGN KEY ("UsuarioId")
        REFERENCES "Usuarios" ("Id") ON DELETE RESTRICT
);
