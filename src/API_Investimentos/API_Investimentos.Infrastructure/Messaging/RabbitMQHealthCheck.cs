using API_Investimentos.Application.Messaging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace API_Investimentos.Infrastructure.Messaging;

/// <summary>
/// Health check para verificar conexão com RabbitMQ
/// </summary>
public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly RabbitMQSettings _settings;

    public RabbitMQHealthCheck(IOptions<RabbitMQSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ está conectado"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Falha na conexão com RabbitMQ",
                ex));
        }
    }
}
