using System.Collections.Concurrent;

namespace API_Investimentos.Api.Services;

public interface ITelemetriaService
{
    void RegistrarChamada(string endpoint, long tempoRespostaMs);
    TelemetriaResponse ObterTelemetria(DateTime? inicio, DateTime? fim);
}

public class TelemetriaService : ITelemetriaService
{
    private readonly ConcurrentBag<RegistroChamada> _registros = new();

    public void RegistrarChamada(string endpoint, long tempoRespostaMs)
    {
        _registros.Add(new RegistroChamada
        {
            Endpoint = endpoint,
            TempoRespostaMs = tempoRespostaMs,
            DataHora = DateTime.UtcNow
        });
    }

    public TelemetriaResponse ObterTelemetria(DateTime? inicio, DateTime? fim)
    {
        var dataInicio = inicio ?? DateTime.UtcNow.AddDays(-30);
        var dataFim = fim ?? DateTime.UtcNow;

        var registrosFiltrados = _registros
            .Where(r => r.DataHora >= dataInicio && r.DataHora <= dataFim)
            .ToList();

        var servicos = registrosFiltrados
            .GroupBy(r => r.Endpoint)
            .Select(g => new ServicoTelemetria
            {
                Nome = g.Key,
                QuantidadeChamadas = g.Count(),
                MediaTempoRespostaMs = g.Any() ? Math.Round(g.Average(r => r.TempoRespostaMs), 2) : 0
            })
            .OrderByDescending(s => s.QuantidadeChamadas)
            .ToList();

        return new TelemetriaResponse
        {
            Servicos = servicos,
            Periodo = new PeriodoTelemetria
            {
                Inicio = dataInicio.ToString("yyyy-MM-dd"),
                Fim = dataFim.ToString("yyyy-MM-dd")
            }
        };
    }

    private class RegistroChamada
    {
        public string Endpoint { get; set; } = string.Empty;
        public long TempoRespostaMs { get; set; }
        public DateTime DataHora { get; set; }
    }
}

public class TelemetriaResponse
{
    public List<ServicoTelemetria> Servicos { get; set; } = new();
    public PeriodoTelemetria Periodo { get; set; } = new();
}

public class ServicoTelemetria
{
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeChamadas { get; set; }
    public double MediaTempoRespostaMs { get; set; }
}

public class PeriodoTelemetria
{
    public string Inicio { get; set; } = string.Empty;
    public string Fim { get; set; } = string.Empty;
}
