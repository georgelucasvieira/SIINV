using Worker_Simulacao.Consumers;
using Worker_Simulacao.Events;
using Worker_Simulacao.Settings;

namespace Worker_Simulacao;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker_Simulacao iniciado");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer>();

            await consumer.ConsumeAsync<SimulacaoRealizadaEvent>(
                RabbitMQConstants.SimulacoesQueue,
                async (message) =>
                {
                    _logger.LogInformation(
                        "Processando evento SimulacaoRealizada - SimulacaoId: {SimulacaoId}, ClienteId: {ClienteId}, Valor: {Valor}",
                        message.SimulacaoId,
                        message.ClienteId,
                        message.ValorInvestido);







                    await Task.CompletedTask;
                },
                stoppingToken);


            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker_Simulacao cancelado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no Worker_Simulacao");
            throw;
        }
    }
}
