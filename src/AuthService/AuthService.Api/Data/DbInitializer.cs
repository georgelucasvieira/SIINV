using AuthService.Api.Data.Entities;
using AuthService.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Data;

/// <summary>
/// Inicializador do banco de dados com seed do usuário admin
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AuthDbContext>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();


        await context.Database.MigrateAsync();


        var adminExists = await context.Usuarios
            .AnyAsync(u => u.Role == Roles.Admin);

        if (!adminExists)
        {

            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                ?? configuration["AdminSettings:Email"];
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                ?? configuration["AdminSettings:Password"];
            var adminName = Environment.GetEnvironmentVariable("ADMIN_NAME")
                ?? configuration["AdminSettings:Name"]
                ?? "Administrador";


            if (!environment.IsDevelopment())
            {
                if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
                {
                    logger.LogError("Em produção, as variáveis ADMIN_EMAIL e ADMIN_PASSWORD são obrigatórias");
                    throw new InvalidOperationException(
                        "Variáveis de ambiente ADMIN_EMAIL e ADMIN_PASSWORD não configuradas. " +
                        "Configure-as antes de iniciar a aplicação em produção.");
                }
            }
            else
            {

                adminEmail ??= "admin@investimentos.com";
                adminPassword ??= "Admin@123";
            }

            logger.LogInformation("Criando usuário admin...");

            var senhaHash = passwordHasher.Hash(adminPassword);

            var admin = new Usuario(
                nome: adminName,
                email: adminEmail,
                senhaHash: senhaHash,
                role: Roles.Admin
            );

            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();

            logger.LogInformation("Usuário admin criado com sucesso. Email: {Email}", adminEmail);

            if (environment.IsDevelopment())
            {
                logger.LogWarning("DESENVOLVIMENTO: Senha padrão em uso. Não use em produção!");
            }
        }
        else
        {
            logger.LogInformation("Usuário admin já existe, pulando seed");
        }
    }
}
