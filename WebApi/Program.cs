using RetroGames.Services.Interfaces;
using FluentValidation;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Middlewares;
using RetroGames.WebApi.Configuration;

// Carrega as variáveis do arquivo .env.
// TraversePath procura o arquivo nos diretórios superiores.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Lê a string de conexão.
// O ASP.NET Core interpreta ConnectionStrings__Default
// como ConnectionStrings:Default.
string? connectionString =
    builder.Configuration.GetConnectionString("Default");

// Impede que a API inicie sem a configuração do banco.
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ERRO: a string de conexão 'ConnectionStrings:Default' " +
        "não foi configurada. Verifique o arquivo .env."
    );
}

// Registra os repositórios e serviços.
builder.Services.AddRepositorios(connectionString);

builder.Services.AddServicosDeNegocio();

// Registra os controllers.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes => opcoes.InvalidModelStateResponseFactory = contexto =>
        FluentValidationFilter.ModeloInvalido(contexto.ModelState.Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(x => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(x.Key), x => x.Value!.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Valor inválido." : e.ErrorMessage).ToArray())));

// Gera a documentação OpenAPI.
builder.Services.AddOpenApi();
builder.Services.AddSessao(builder.Environment.IsDevelopment());
builder.Services.AddPoliticas();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
using (var escopo = app.Services.CreateScope())
{
    var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacao.GarantirAdministradorInicialAsync(builder.Configuration["AdminInicial:Email"], builder.Configuration["AdminInicial:Senha"]);
}

app.UseExceptionHandler();

// Swagger disponível apenas em desenvolvimento.
if (app.Environment.IsDevelopment())
{
    // Documento OpenAPI em /openapi/v1.json.
    app.MapOpenApi().AllowAnonymous();

    // Interface Swagger em /swagger.
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "RetroGames API v1"
        );

        options.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Mapeia as rotas dos controllers.
app.MapControllers();

app.Run();
