using API_Investimentos.Domain.Entities;

namespace API_Investimentos.Domain.Interfaces;

/// <summary>
/// Repositório para operações com clientes
/// </summary>
public interface IRepositorioCliente : IRepositorioBase<Cliente>
{
    /// <summary>
    /// Obtém um cliente pelo CPF
    /// </summary>
    Task<Cliente?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe um cliente com o CPF informado
    /// </summary>
    Task<bool> ExisteCpfAsync(string cpf, CancellationToken cancellationToken = default);
}
