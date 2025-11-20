using API_Investimentos.Api.Middleware;
using API_Investimentos.Application;
using API_Investimentos.Infrastructure;
using API_Investimentos.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Adicionar camadas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Simulação de Investimentos",
        Version = "v1",
        Description = "API para simulação de investimentos financeiros (CDB, Tesouro Direto, LCI, LCA, Fundos, etc.)",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@investimentos.com"
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Incluir comentários XML da Application
    var applicationXmlFile = "API_Investimentos.Application.xml";
    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
    if (File.Exists(applicationXmlPath))
    {
        c.IncludeXmlComments(applicationXmlPath);
    }
});

// CORS (se necessário)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Inicializar banco de dados e seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<InvestimentosDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        await DbInitializer.InitializeAsync(context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao inicializar o banco de dados");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Investimentos v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

// Middleware de tratamento de exceções
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Logging de requisições
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Iniciando API de Investimentos");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class public so integration tests can access it
public partial class Program { }
