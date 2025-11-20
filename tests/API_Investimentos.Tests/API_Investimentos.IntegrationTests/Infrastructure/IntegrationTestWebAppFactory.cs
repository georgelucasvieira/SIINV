using API_Investimentos.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API_Investimentos.IntegrationTests.Infrastructure;

/// <summary>
/// Factory customizada para testes de integração
/// </summary>
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public IntegrationTestWebAppFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remover o DbContext existente
            services.RemoveAll(typeof(DbContextOptions<InvestimentosDbContext>));

            // Adicionar DbContext com SQL Server de teste
            services.AddDbContext<InvestimentosDbContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });

            // Criar banco de dados e aplicar migrations
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InvestimentosDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
        });
    }
}
