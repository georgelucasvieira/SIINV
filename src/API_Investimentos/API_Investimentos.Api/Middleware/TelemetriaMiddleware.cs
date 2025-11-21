using System.Diagnostics;
using API_Investimentos.Api.Services;

namespace API_Investimentos.Api.Middleware;

public class TelemetriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITelemetriaService _telemetriaService;


    private static readonly HashSet<string> EndpointsRastreados = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/simular-investimento",
        "/api/v1/simulacoes",
        "/api/v1/simulacoes/por-produto-dia",
        "/api/v1/perfil-risco",
        "/api/v1/produtos",
        "/api/v1/produtos/recomendados",
        "/api/v1/investimentos",
        "/api/v1/clientes"
    };

    public TelemetriaMiddleware(RequestDelegate next, ITelemetriaService telemetriaService)
    {
        _next = next;
        _telemetriaService = telemetriaService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;


        var endpointRastreado = EndpointsRastreados
            .FirstOrDefault(e => path.StartsWith(e, StringComparison.OrdinalIgnoreCase));

        if (endpointRastreado == null)
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();


            var nomeServico = ExtrairNomeServico(path);
            _telemetriaService.RegistrarChamada(nomeServico, stopwatch.ElapsedMilliseconds);
        }
    }

    private static string ExtrairNomeServico(string path)
    {

        var partes = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (partes.Length >= 3)
        {

            return string.Join("-", partes.Skip(2));
        }

        return path.TrimStart('/').Replace('/', '-');
    }
}
