namespace API_Investimentos.Domain.Enums;

/// <summary>
/// Nível de risco do produto de investimento
/// </summary>
public enum NivelRisco
{
    /// <summary>
    /// Risco muito baixo (ex: Tesouro Direto, CDB com garantia do FGC)
    /// </summary>
    MuitoBaixo = 1,

    /// <summary>
    /// Risco baixo (ex: LCI/LCA, CDB de bancos sólidos)
    /// </summary>
    Baixo = 2,

    /// <summary>
    /// Risco médio (ex: Fundos de Renda Fixa, Debêntures)
    /// </summary>
    Medio = 3,

    /// <summary>
    /// Risco alto (ex: Fundos Multimercado, Ações)
    /// </summary>
    Alto = 4,

    /// <summary>
    /// Risco muito alto (ex: Derivativos, Mercados emergentes)
    /// </summary>
    MuitoAlto = 5
}
