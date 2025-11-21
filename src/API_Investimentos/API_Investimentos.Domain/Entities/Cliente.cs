using API_Investimentos.Domain.Common;

namespace API_Investimentos.Domain.Entities;

/// <summary>
/// Entidade que representa um cliente
/// </summary>
public class Cliente : BaseEntity
{
    /// <summary>
    /// Nome do cliente
    /// </summary>
    public string Nome { get; private set; }

    /// <summary>
    /// CPF do cliente
    /// </summary>
    public string Cpf { get; private set; }

    /// <summary>
    /// Telefone do cliente
    /// </summary>
    public string Telefone { get; private set; }

    // Construtor privado para EF Core
    private Cliente()
    {
        Nome = string.Empty;
        Cpf = string.Empty;
        Telefone = string.Empty;
    }

    public Cliente(string nome, string cpf, string telefone)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF é obrigatório", nameof(cpf));

        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório", nameof(telefone));

        Nome = nome;
        Cpf = LimparCpf(cpf);
        Telefone = telefone;
    }

    public void Atualizar(string nome, string telefone)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório", nameof(telefone));

        Nome = nome;
        Telefone = telefone;
        MarcarComoAtualizado();
    }

    private static string LimparCpf(string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }
}
