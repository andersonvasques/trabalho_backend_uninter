using System.Text;
using System.Text.Json.Serialization;
using Franquias.Api.Configurations;
using Franquias.Api.Data;
using Franquias.Api.Middleware;
using Franquias.Api.Repositories;
using Franquias.Api.Security;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api;

/// <summary>
/// Ponto de entrada da API de gestão de franquias.
///
/// Aqui são registrados os serviços da aplicação (injeção de dependência),
/// o banco de dados, a autenticação JWT, a documentação OpenAPI/Swagger
/// e o pipeline de middlewares.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // =================================================================
        // Controllers e serialização JSON
        // =================================================================
        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                // Os enums aparecem como texto no JSON ("Ativa" em vez de 2).
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                // Evita erro de referência cíclica entre entidades relacionadas.
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        // Documento OpenAPI nativo do ASP.NET Core.
        builder.Services.AddOpenApi();

        // Respostas de erro padronizadas (ProblemDetails).
        builder.Services.AddProblemDetails();

        // Necessário para que os serviços consigam ler as claims do usuário logado.
        builder.Services.AddHttpContextAccessor();

        // =================================================================
        // Banco de dados relacional (SQLite + Entity Framework Core)
        // =================================================================
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "A string de conexão 'DefaultConnection' não foi encontrada.");

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        // =================================================================
        // Autenticação e autorização com JWT
        // =================================================================
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection(JwtOptions.Secao));

        JwtOptions opcoesJwt = builder.Configuration
            .GetSection(JwtOptions.Secao)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "A seção de configuração 'Jwt' não foi encontrada.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = opcoesJwt.Emissor,
                    ValidAudience = opcoesJwt.Audiencia,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(opcoesJwt.Chave)),
                    NameClaimType = System.Security.Claims.ClaimTypes.Name,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        builder.Services.AddAuthorization();

        // =================================================================
        // Injeção de dependência: repositórios
        // =================================================================
        builder.Services.AddScoped(typeof(IRepositorioBase<>), typeof(RepositorioBase<>));
        builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        builder.Services.AddScoped<IUnidadeRepositorio, UnidadeRepositorio>();
        builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
        builder.Services.AddScoped<IFornecedorRepositorio, FornecedorRepositorio>();
        builder.Services.AddScoped<IEstoqueRepositorio, EstoqueRepositorio>();
        builder.Services.AddScoped<IVendaRepositorio, VendaRepositorio>();
        builder.Services.AddScoped<IRoyaltyRepositorio, RoyaltyRepositorio>();
        builder.Services.AddScoped<IChamadoRepositorio, ChamadoRepositorio>();
        builder.Services.AddScoped<IRelatorioRepositorio, RelatorioRepositorio>();

        // =================================================================
        // Injeção de dependência: segurança e serviços de negócio
        // =================================================================
        builder.Services.AddScoped<IGeradorDeToken, GeradorDeToken>();
        builder.Services.AddScoped<IUsuarioContexto, UsuarioContexto>();

        builder.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<IRedeService, RedeService>();
        builder.Services.AddScoped<IUnidadeService, UnidadeService>();
        builder.Services.AddScoped<ICategoriaService, CategoriaService>();
        builder.Services.AddScoped<IProdutoService, ProdutoService>();
        builder.Services.AddScoped<IFornecedorService, FornecedorService>();
        builder.Services.AddScoped<IEstoqueService, EstoqueService>();
        builder.Services.AddScoped<IVendaService, VendaService>();
        builder.Services.AddScoped<IRoyaltyService, RoyaltyService>();
        builder.Services.AddScoped<IChamadoService, ChamadoService>();
        builder.Services.AddScoped<IRelatorioService, RelatorioService>();

        WebApplication app = builder.Build();

        // =================================================================
        // Criação do banco e carga de dados de exemplo
        // =================================================================
        using (IServiceScope escopo = app.Services.CreateScope())
        {
            AppDbContext contexto = escopo.ServiceProvider.GetRequiredService<AppDbContext>();

            // Quando existem migrations no projeto elas são aplicadas;
            // caso contrário o banco é criado diretamente a partir do modelo.
            if (contexto.Database.GetMigrations().Any())
            {
                await contexto.Database.MigrateAsync();
            }
            else
            {
                await contexto.Database.EnsureCreatedAsync();
            }

            bool seedHabilitado = app.Configuration.GetValue<bool>("Seed:Habilitado");

            if (seedHabilitado)
            {
                await SeedDados.PopularAsync(contexto);
            }
        }

        // =================================================================
        // Pipeline de middlewares
        // =================================================================
        app.UseMiddleware<TratamentoDeErrosMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            // Documento OpenAPI em /openapi/v1.json
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Franquias API v1");
                options.DocumentTitle = "API de Gestão de Franquias";

                // Guarda automaticamente o token devolvido pelo /api/auth/login...
                options.UseResponseInterceptor(
                    "function (res) { try { if (res.url && res.url.indexOf('/api/auth/login') !== -1 && res.status === 200) { " +
                    "var corpo = (typeof res.body === 'string') ? JSON.parse(res.body) : res.body; " +
                    "if (corpo && corpo.token) { window.localStorage.setItem('franquias_token', corpo.token); } } } catch (e) { } return res; }");

                // ...e envia esse token no cabeçalho Authorization das demais chamadas.
                options.UseRequestInterceptor(
                    "function (req) { try { var token = window.localStorage.getItem('franquias_token'); " +
                    "if (token && req.url.indexOf('/api/auth/login') === -1) { req.headers['Authorization'] = 'Bearer ' + token; } } catch (e) { } return req; }");
            });

            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        }
        else
        {
            app.MapGet("/", () => Results.Ok(new
            {
                aplicacao = "Franquias.Api",
                status = "OnLine"
            })).ExcludeFromDescription();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        // Verificação simples de saúde da aplicação.
        app.MapGet("/health", () => Results.Ok(new
        {
            status = "ok",
            aplicacao = "Franquias.Api",
            dataHoraUtc = DateTime.UtcNow
        })).WithName("Saude").WithTags("Infraestrutura");

        app.MapControllers();

        await app.RunAsync();
    }
}
