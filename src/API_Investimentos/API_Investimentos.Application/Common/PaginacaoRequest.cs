namespace API_Investimentos.Application.Common;

/// <summary>
/// Parâmetros para paginação de resultados
/// </summary>
public class PaginacaoRequest
{
    private const int MaxTamanhoPagina = 100;
    private int _tamanhoPagina = 10;

    /// <summary>
    /// Número da página (inicia em 1)
    /// </summary>
    public int NumeroPagina { get; set; } = 1;

    /// <summary>
    /// Tamanho da página (máximo 100)
    /// </summary>
    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value > MaxTamanhoPagina ? MaxTamanhoPagina : value;
    }

    /// <summary>
    /// Calcula o número de registros a pular
    /// </summary>
    public int Skip => (NumeroPagina - 1) * TamanhoPagina;
}
