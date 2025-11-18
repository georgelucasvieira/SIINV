using API_Investimentos.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace API_Investimentos.Application;

/// <summary>
/// Configuração de injeção de dependência da camada Application
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper
        services.AddAutoMapper(assembly);

        // Services
        services.AddScoped<ICalculadoraInvestimentos, CalculadoraInvestimentos>();

        return services;
    }
}
