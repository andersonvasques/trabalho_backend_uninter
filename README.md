# Sistema de Gestão de Franquias — API REST

Trabalho acadêmico da disciplina **Desenvolvimento Web Back-end** (UNINTER).

API REST em C# / ASP.NET Core para gerenciar uma rede de franquias: usuários e perfis de
acesso, unidades franqueadas, franqueados, catálogo de produtos e serviços, fornecedores,
estoque por unidade, vendas, royalties, chamados de suporte e indicadores gerenciais.

---

## Tecnologias

C# (.NET 10) · ASP.NET Core Web API · Entity Framework Core 10 · SQLite ·
JWT Bearer · PBKDF2-SHA256 para senhas · OpenAPI + Swagger UI

## Como executar

```bash
cd Franquias.Api
dotnet restore
dotnet run
```

- Swagger: <http://localhost:5210/swagger> · Health: <http://localhost:5210/health>
- O banco `Franquias.db` é criado e populado automaticamente na primeira execução
  (5 unidades, 10 produtos, 3 fornecedores, ~56 vendas, royalties e chamados).
- Para recomeçar do zero: apague `Franquias.db` e rode novamente.

**Migrations (opcional).** O projeto cria o banco pelo modelo quando não há migrations.
Para versioná-las: `dotnet tool install --global dotnet-ef`, apague `Franquias.db` e rode
`dotnet ef migrations add InitialCreate` + `dotnet ef database update`. A partir daí a
aplicação passa a aplicar as migrations no start. O DDL equivalente está em
`Franquias.Api/Scripts/criar-banco.sql`.

## Acesso

Senha de todos os usuários: **`Senha@123`**

| E-mail | Perfil | Unidade |
|---|---|---|
| `admin@saborecia.com.br` | Administrador | — (franqueadora) |
| `gestor.batel@saborecia.com.br` | Gestor | UN-001 Batel |
| `operador.batel@saborecia.com.br` | Operador | UN-001 Batel |

Há também gestores para as unidades UN-002, UN-003 e UN-004 (`gestor.londrina@`,
`gestor.pinheiros@`, `gestor.moinhos@`).

No Swagger, execute `POST /api/auth/login`: o token é guardado automaticamente e enviado
no cabeçalho `Authorization` das demais requisições. Fora do Swagger, envie
`Authorization: Bearer <token>`.

**Perfis:** *Administrador* administra toda a rede; *Gestor* e *Operador* só acessam a
própria unidade — o vínculo viaja no token (claim `unidadeId`) e o acesso a outra unidade
devolve **403**.

## Estrutura

```
Franquias.Api/
├── Controllers/     13 controllers REST          ├── Models/          entidades do domínio
├── Services/        regras de negócio            ├── DTOs/            entrada e saída
├── Repositories/    acesso a dados (EF Core)     ├── Configurations/  mapeamento EF + JWT
├── Data/            DbContext e carga inicial    ├── Middleware/      erros e log
├── Security/        JWT, hash de senha, contexto ├── Validations/     CPF, CNPJ, campos
└── Scripts/         DDL do banco                                       opcionais
```

`Controller → Service (regras) → Repository (EF Core) → SQLite`, com DTOs na entrada e na saída.

## Endpoints

| Grupo | Rotas |
|---|---|
| Autenticação | `POST /api/auth/login` · `GET /api/auth/eu` · `PUT /api/auth/senha` |
| Cadastros | `/api/usuarios` · `/api/franqueadoras` · `/api/franqueados` · `/api/unidades` · `/api/categorias` · `/api/produtos` · `/api/fornecedores` (GET, POST, PUT, PATCH `/ativar`, DELETE = inativar) |
| Fornecimento | `POST` e `DELETE /api/fornecedores/{id}/produtos` |
| Estoque | `GET /api/estoques` · `/criticos` · `/unidades/{u}/produtos/{p}` · `POST` e `GET /movimentacoes` · `PUT /minimo` |
| Vendas | `GET`/`POST /api/vendas` · `PATCH /api/vendas/{id}/cancelar` |
| Royalties | `GET /api/royalties` · `POST /apuracoes` · `PUT /{id}/pagamento` · `DELETE /{id}` |
| Chamados | `GET`/`POST /api/chamados` · `POST /{id}/interacoes` · `PUT /{id}/encerramento` |
| Relatórios | `/api/relatorios/` `faturamento` · `ranking-unidades` · `produtos-mais-vendidos` · `estoque-critico` · `royalties` · `chamados-por-status` · `chamados-por-prioridade` · `painel` |

Todas as listagens aceitam `pagina`, `tamanhoPagina` (máx. 100), `ordenarPor` e
`decrescente`, além dos filtros de cada módulo. O arquivo `Franquias.Api.http` traz 50
requisições prontas, incluindo os casos de erro.

## Regras de negócio

1. CNPJ único por unidade e e-mail único por usuário (validação + índice no banco).
2. Unidade inativa não registra vendas.
3. Venda pertence a uma única unidade e exige ao menos um item.
4. Total da venda calculado a partir dos itens, quantidades, preços e desconto.
5. Estoque nunca fica negativo; a venda dá baixa e o cancelamento devolve os itens.
6. Royalty = faturamento confirmado do período × percentual da unidade.
7. Cobrança paga ou cancelada não é recalculada; pagamento não excede o saldo devedor.
8. Cada perfil só executa as operações permitidas, restritas à sua unidade.
9. Nada é apagado fisicamente: status ativo/inativo e histórico (movimentações, interações).
10. Serviços não controlam estoque; CPF e CNPJ validados por dígito verificador.
11. Erros devolvem 400, 401, 403, 404, 409 ou 500 em formato `ProblemDetails`.

## Decisões técnicas

- **Camadas separadas:** controllers finos, regras nos services, consultas nos repositories
  (genérico `RepositorioBase<T>` + repositórios específicos com filtros e paginação).
- **DTOs** em todas as entradas e saídas — as entidades nunca são expostas.
- **`async/await`** em todo o acesso a dados, com `CancellationToken` ponta a ponta.
- **Exceções de domínio** (`RegraDeNegocioException`, `NaoEncontradoException`,
  `ConflitoException`, `AcessoNegadoException`) convertidas em `ProblemDetails` por um
  middleware único.
- **Senhas** com PBKDF2 + SHA-256, salt por usuário e comparação em tempo fixo.
- **Valores monetários** são `decimal` no C# e convertidos para `double` no provedor
  SQLite (`HasConversion<double>()`), já que o SQLite não tem tipo decimal nativo — sem
  isso, somas e ordenações por valor não funcionam no LINQ.
- **Enums gravados como texto**, deixando o banco legível para conferência.
