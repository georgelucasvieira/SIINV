namespace API_Investimentos.Application.Messaging;

/// <summary>
/// Configurações do RabbitMQ
/// </summary>
public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
}

/// <summary>
/// Constantes para nomes de filas e exchanges
/// </summary>
public static class RabbitMQConstants
{
    // Exchanges
    public const string InvestimentosExchange = "investimentos.exchange";

    // Queues
    public const string SimulacoesQueue = "simulacoes.queue";
    public const string ClientesQueue = "clientes.queue";
    public const string NotificacoesQueue = "notificacoes.queue";

    // Routing Keys
    public const string SimulacaoRealizadaRoutingKey = "simulacao.realizada";
    public const string ClienteCriadoRoutingKey = "cliente.criado";
    public const string NotificacaoEmailRoutingKey = "notificacao.email";
}
