using API_Investimentos.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Xunit;

namespace API_Investimentos.IntegrationTests.Infrastructure;

/// <summary>
/// Classe base para testes de integração com Testcontainers
/// </summary>
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;
    protected IntegrationTestWebAppFactory Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;

    protected BaseIntegrationTest()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong@Passw0rd")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        var connectionString = _msSqlContainer.GetConnectionString();
        Factory = new IntegrationTestWebAppFactory(connectionString);
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        Factory?.Dispose();
        await _msSqlContainer.DisposeAsync();
    }

    /// <summary>
    /// Obtém o DbContext para operações diretas no banco de dados durante os testes
    /// </summary>
    protected InvestimentosDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<InvestimentosDbContext>();
    }
}
