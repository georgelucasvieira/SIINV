using System.Text;
using System.Text.Json;
using API_Investimentos.Application.Messaging;
using API_Investimentos.Application.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace API_Investimentos.Infrastructure.Messaging;

/// <summary>
/// Implementação do consumer usando RabbitMQ
/// </summary>
public class RabbitMQConsumer : IMessageConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private readonly RabbitMQSettings _settings;
    private bool _disposed;

    public RabbitMQConsumer(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQConsumer> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();


        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _logger.LogInformation("RabbitMQ Consumer inicializado com sucesso");
    }

    public Task ConsumeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<T>(json);

                if (message != null)
                {
                    _logger.LogInformation("Mensagem recebida da fila {QueueName}: {MessageType}", queueName, typeof(T).Name);

                    await handler(message);

                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Mensagem processada com sucesso");
                }
                else
                {
                    _logger.LogWarning("Mensagem recebida da fila {QueueName} é nula após deserialização", queueName);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem da fila {QueueName}", queueName);


                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Consumidor iniciado para fila {QueueName}", queueName);


        cancellationToken.Register(() =>
        {
            _logger.LogInformation("Cancelando consumidor da fila {QueueName}", queueName);
        });

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        _disposed = true;
    }
}
