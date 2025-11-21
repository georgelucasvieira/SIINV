using API_Investimentos.Application.Messaging;
using API_Investimentos.Application.Messaging.Interfaces;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Infrastructure.Data;
using API_Investimentos.Infrastructure.Messaging;
using API_Investimentos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API_Investimentos.Infrastructure;

/// <summary>
/// Configuração de injeção de dependência da camada Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<InvestimentosDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(InvestimentosDbContext).Assembly.FullName)));

        // Repositórios
        services.AddScoped<IRepositorioProduto, RepositorioProduto>();
        services.AddScoped<IRepositorioSimulacao, RepositorioSimulacao>();
        services.AddScoped<IRepositorioPerfilRisco, RepositorioPerfilRisco>();
        services.AddScoped<IRepositorioHistoricoInvestimento, RepositorioHistoricoInvestimento>();
        services.AddScoped<IRepositorioCliente, RepositorioCliente>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // RabbitMQ (apenas Publisher - Consumer está no Worker_Simulacao)
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

        return services;
    }
}
