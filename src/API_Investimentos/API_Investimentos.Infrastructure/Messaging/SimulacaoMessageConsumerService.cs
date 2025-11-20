using API_Investimentos.Application.Messaging;
using API_Investimentos.Application.Messaging.Events;
using API_Investimentos.Application.Messaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API_Investimentos.Infrastructure.Messaging;

/// <summary>
/// Background service para consumir mensagens de simulação
/// </summary>
public class SimulacaoMessageConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SimulacaoMessageConsumerService> _logger;

    public SimulacaoMessageConsumerService(
        IServiceProvider serviceProvider,
        ILogger<SimulacaoMessageConsumerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SimulacaoMessageConsumerService iniciado");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();

            await consumer.ConsumeAsync<SimulacaoRealizadaEvent>(
                RabbitMQConstants.SimulacoesQueue,
                async (message) =>
                {
                    _logger.LogInformation(
                        "Processando evento SimulacaoRealizada - SimulacaoId: {SimulacaoId}, ClienteId: {ClienteId}, Valor: {Valor}",
                        message.SimulacaoId,
                        message.ClienteId,
                        message.ValorInvestido);

                    // Aqui você pode adicionar lógica adicional como:
                    // - Enviar notificação por email
                    // - Atualizar estatísticas
                    // - Registrar em um sistema de analytics
                    // - Notificar outros serviços

                    await Task.CompletedTask;
                },
                stoppingToken);

            // Mantém o serviço rodando
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("SimulacaoMessageConsumerService cancelado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no SimulacaoMessageConsumerService");
            throw;
        }
    }
}
