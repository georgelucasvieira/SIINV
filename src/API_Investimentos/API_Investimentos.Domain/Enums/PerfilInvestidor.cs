namespace API_Investimentos.Domain.Enums;

/// <summary>
/// Perfil de risco do investidor
/// </summary>
public enum PerfilInvestidor
{
    /// <summary>
    /// Conservador: pontuação 0-35, prefere liquidez e segurança
    /// </summary>
    Conservador = 1,

    /// <summary>
    /// Moderado: pontuação 36-65, equilíbrio entre risco e retorno
    /// </summary>
    Moderado = 2,

    /// <summary>
    /// Agressivo: pontuação 66-100, busca alta rentabilidade aceitando maior risco
    /// </summary>
    Agressivo = 3
}
