using API_Investimentos.Domain.Common;
using API_Investimentos.Domain.Enums;

namespace API_Investimentos.Domain.Entities;

/// <summary>
/// Entidade que representa o perfil de risco de um cliente
/// </summary>
public class PerfilRisco : BaseEntity
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public long ClienteId { get; private set; }

    /// <summary>
    /// Perfil calculado do investidor
    /// </summary>
    public PerfilInvestidor Perfil { get; private set; }

    /// <summary>
    /// Pontuação calculada (0-100)
    /// </summary>
    public int Pontuacao { get; private set; }

    /// <summary>
    /// Descrição textual do perfil
    /// </summary>
    public string Descricao { get; private set; }

    /// <summary>
    /// Fatores de cálculo armazenados como JSON
    /// </summary>
    public string? FatoresCalculo { get; private set; }

    /// <summary>
    /// Data da próxima reavaliação
    /// </summary>
    public DateTime DataProximaAvaliacao { get; private set; }


    private PerfilRisco()
    {
        Descricao = string.Empty;
    }

    public PerfilRisco(
        long clienteId,
        int pontuacao,
        string? fatoresCalculo = null)
    {
        if (clienteId <= 0)
            throw new ArgumentException("ClienteId inválido", nameof(clienteId));

        if (pontuacao < 0 || pontuacao > 100)
            throw new ArgumentException("Pontuação deve estar entre 0 e 100", nameof(pontuacao));

        ClienteId = clienteId;
        Pontuacao = pontuacao;
        Perfil = CalcularPerfil(pontuacao);
        Descricao = ObterDescricaoPerfil(Perfil);
        FatoresCalculo = fatoresCalculo;
        DataProximaAvaliacao = DateTime.UtcNow.AddMonths(3); // Reavaliação a cada 3 meses
    }

    public void AtualizarPontuacao(int novaPontuacao, string? fatoresCalculo = null)
    {
        if (novaPontuacao < 0 || novaPontuacao > 100)
            throw new ArgumentException("Pontuação deve estar entre 0 e 100", nameof(novaPontuacao));

        Pontuacao = novaPontuacao;
        Perfil = CalcularPerfil(novaPontuacao);
        Descricao = ObterDescricaoPerfil(Perfil);
        FatoresCalculo = fatoresCalculo;
        DataProximaAvaliacao = DateTime.UtcNow.AddMonths(3);

        MarcarComoAtualizado();
    }

    public bool PrecisaReavaliar() => DateTime.UtcNow >= DataProximaAvaliacao;

    private static PerfilInvestidor CalcularPerfil(int pontuacao)
    {
        return pontuacao switch
        {
            <= 35 => PerfilInvestidor.Conservador,
            <= 65 => PerfilInvestidor.Moderado,
            _ => PerfilInvestidor.Agressivo
        };
    }

    private static string ObterDescricaoPerfil(PerfilInvestidor perfil)
    {
        return perfil switch
        {
            PerfilInvestidor.Conservador => "Perfil equilibrado entre segurança e rentabilidade. Prefere investimentos de baixo risco com liquidez.",
            PerfilInvestidor.Moderado => "Perfil equilibrado entre segurança e rentabilidade. Aceita algum risco em busca de retornos maiores.",
            PerfilInvestidor.Agressivo => "Perfil arrojado que busca alta rentabilidade. Aceita maior volatilidade e riscos em busca de melhores retornos.",
            _ => "Perfil não definido"
        };
    }
}
