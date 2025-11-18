namespace API_Investimentos.Domain.Enums;

/// <summary>
/// Tipo de produto de investimento
/// </summary>
public enum TipoProduto
{
    /// <summary>
    /// Certificado de Depósito Bancário
    /// </summary>
    CDB = 1,

    /// <summary>
    /// Tesouro Direto - Selic (pós-fixado)
    /// </summary>
    TesouroSelic = 2,

    /// <summary>
    /// Tesouro Direto - Prefixado
    /// </summary>
    TesouroPrefixado = 3,

    /// <summary>
    /// Tesouro Direto - IPCA+ (híbrido)
    /// </summary>
    TesouroIPCA = 4,

    /// <summary>
    /// Letra de Crédito Imobiliário
    /// </summary>
    LCI = 5,

    /// <summary>
    /// Letra de Crédito do Agronegócio
    /// </summary>
    LCA = 6,

    /// <summary>
    /// Fundo de Investimento (Renda Fixa, Multimercado, Ações, etc.)
    /// </summary>
    Fundo = 7
}
