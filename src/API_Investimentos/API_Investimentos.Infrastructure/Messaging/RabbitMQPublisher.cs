using System.Text;
using System.Text.Json;
using API_Investimentos.Application.Messaging;
using API_Investimentos.Application.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace API_Investimentos.Infrastructure.Messaging;

/// <summary>
/// Implementação do publisher usando RabbitMQ
/// </summary>
public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly RabbitMQSettings _settings;
    private bool _disposed;

    public RabbitMQPublisher(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQPublisher> logger)
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

        // Declarar exchange principal
        _channel.ExchangeDeclare(
            exchange: RabbitMQConstants.InvestimentosExchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        // Declarar filas
        DeclareQueue(RabbitMQConstants.SimulacoesQueue, RabbitMQConstants.SimulacaoRealizadaRoutingKey);
        DeclareQueue(RabbitMQConstants.ClientesQueue, RabbitMQConstants.ClienteCriadoRoutingKey);
        DeclareQueue(RabbitMQConstants.NotificacoesQueue, RabbitMQConstants.NotificacaoEmailRoutingKey);

        _logger.LogInformation("RabbitMQ Publisher inicializado com sucesso");
    }

    private void DeclareQueue(string queueName, string routingKey)
    {
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueBind(
            queue: queueName,
            exchange: RabbitMQConstants.InvestimentosExchange,
            routingKey: routingKey);
    }

    public Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Mensagem publicada na fila {QueueName}: {MessageType}", queueName, typeof(T).Name);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem na fila {QueueName}", queueName);
            throw;
        }
    }

    public Task PublishToExchangeAsync<T>(T message, string exchangeName, string routingKey, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Mensagem publicada no exchange {ExchangeName} com routing key {RoutingKey}: {MessageType}",
                exchangeName, routingKey, typeof(T).Name);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no exchange {ExchangeName}", exchangeName);
            throw;
        }
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
