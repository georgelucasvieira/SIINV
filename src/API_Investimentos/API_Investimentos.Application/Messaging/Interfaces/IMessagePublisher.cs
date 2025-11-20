namespace API_Investimentos.Application.Messaging.Interfaces;

/// <summary>
/// Interface para publicação de mensagens no message broker
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publica uma mensagem em uma fila específica
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="message">Mensagem a ser publicada</param>
    /// <param name="queueName">Nome da fila</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publica uma mensagem em um exchange com routing key
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="message">Mensagem a ser publicada</param>
    /// <param name="exchangeName">Nome do exchange</param>
    /// <param name="routingKey">Routing key</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task PublishToExchangeAsync<T>(T message, string exchangeName, string routingKey, CancellationToken cancellationToken = default) where T : class;
}
