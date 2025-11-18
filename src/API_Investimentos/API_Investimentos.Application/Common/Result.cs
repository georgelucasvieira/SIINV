namespace API_Investimentos.Application.Common;

/// <summary>
/// Representa o resultado de uma operação
/// </summary>
public class Result
{
    public bool Sucesso { get; }
    public string? Mensagem { get; }
    public List<string> Erros { get; }

    protected Result(bool sucesso, string? mensagem, List<string>? erros = null)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
        Erros = erros ?? new List<string>();
    }

    public static Result Ok(string? mensagem = null) => new Result(true, mensagem);

    public static Result Falha(string mensagem) => new Result(false, mensagem);

    public static Result Falha(List<string> erros) => new Result(false, null, erros);
}

/// <summary>
/// Representa o resultado de uma operação com valor de retorno
/// </summary>
public class Result<T> : Result
{
    public T? Dados { get; }

    protected Result(bool sucesso, T? dados, string? mensagem, List<string>? erros = null)
        : base(sucesso, mensagem, erros)
    {
        Dados = dados;
    }

    public static Result<T> Ok(T dados, string? mensagem = null) => new Result<T>(true, dados, mensagem);

    public new static Result<T> Falha(string mensagem) => new Result<T>(false, default, mensagem);

    public new static Result<T> Falha(List<string> erros) => new Result<T>(false, default, null, erros);
}
