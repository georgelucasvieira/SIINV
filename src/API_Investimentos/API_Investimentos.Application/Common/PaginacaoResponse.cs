namespace API_Investimentos.Application.Common;

/// <summary>
/// Resposta paginada
/// </summary>
public class PaginacaoResponse<T>
{
    public List<T> Itens { get; set; } = new();
    public int NumeroPagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamanhoPagina);
    public bool TemPaginaAnterior => NumeroPagina > 1;
    public bool TemProximaPagina => NumeroPagina < TotalPaginas;

    public PaginacaoResponse()
    {
    }

    public PaginacaoResponse(List<T> itens, int totalRegistros, int numeroPagina, int tamanhoPagina)
    {
        Itens = itens;
        TotalRegistros = totalRegistros;
        NumeroPagina = numeroPagina;
        TamanhoPagina = tamanhoPagina;
    }
}
