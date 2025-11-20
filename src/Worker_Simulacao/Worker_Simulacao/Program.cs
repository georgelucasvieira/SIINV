using Serilog;
using Worker_Simulacao;
using Worker_Simulacao.Consumers;
using Worker_Simulacao.Settings;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando Worker_Simulacao");

    var builder = Host.CreateApplicationBuilder(args);

    // Configurar Serilog
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // Configurar RabbitMQ
    builder.Services.Configure<RabbitMQSettings>(
        builder.Configuration.GetSection("RabbitMQ"));

    // Registrar serviços
    builder.Services.AddScoped<RabbitMQConsumer>();

    // Registrar Worker
    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker_Simulacao falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}
