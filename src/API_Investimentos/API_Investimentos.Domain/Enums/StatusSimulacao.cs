namespace API_Investimentos.Domain.Enums;

/// <summary>
/// Status da simulação de investimento
/// </summary>
public enum StatusSimulacao
{
    /// <summary>
    /// Simulação criada mas ainda não processada
    /// </summary>
    Pendente = 1,

    /// <summary>
    /// Simulação processada com sucesso
    /// </summary>
    Concluida = 2,

    /// <summary>
    /// Simulação com erro no processamento
    /// </summary>
    Erro = 3,

    /// <summary>
    /// Simulação cancelada
    /// </summary>
    Cancelada = 4
}
