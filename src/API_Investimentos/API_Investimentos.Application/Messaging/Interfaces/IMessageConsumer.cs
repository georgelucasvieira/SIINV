namespace API_Investimentos.Application.Messaging.Interfaces;

/// <summary>
/// Interface para consumo de mensagens do message broker
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Inicia o consumo de mensagens de uma fila
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="queueName">Nome da fila</param>
    /// <param name="handler">Handler para processar a mensagem</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task ConsumeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class;
}
