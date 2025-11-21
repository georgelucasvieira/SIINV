using API_Investimentos.Domain.Common;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;

namespace API_Investimentos.Domain.Entities;

/// <summary>
/// Entidade que representa o histórico de investimentos de um cliente
/// </summary>
public class HistoricoInvestimento : BaseEntity
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public long ClienteId { get; private set; }

    /// <summary>
    /// Tipo do produto investido
    /// </summary>
    public TipoProduto TipoProduto { get; private set; }

    /// <summary>
    /// Valor investido
    /// </summary>
    public Dinheiro Valor { get; private set; }

    /// <summary>
    /// Taxa de rentabilidade contratada
    /// </summary>
    public Percentual Rentabilidade { get; private set; }

    /// <summary>
    /// Data do investimento
    /// </summary>
    public DateTime DataInvestimento { get; private set; }

    /// <summary>
    /// Data de vencimento
    /// </summary>
    public DateTime? DataVencimento { get; private set; }

    /// <summary>
    /// Indica se foi resgatado
    /// </summary>
    public bool Resgatado { get; private set; }

    /// <summary>
    /// Data do resgate
    /// </summary>
    public DateTime? DataResgate { get; private set; }


    private HistoricoInvestimento()
    {
        Valor = Dinheiro.Zero;
        Rentabilidade = Percentual.Zero;
    }

    public HistoricoInvestimento(
        long clienteId,
        TipoProduto tipoProduto,
        Dinheiro valor,
        Percentual rentabilidade,
        DateTime dataInvestimento,
        DateTime? dataVencimento = null)
    {
        if (clienteId <= 0)
            throw new ArgumentException("ClienteId inválido", nameof(clienteId));

        ClienteId = clienteId;
        TipoProduto = tipoProduto;
        Valor = valor;
        Rentabilidade = rentabilidade;
        DataInvestimento = dataInvestimento;
        DataVencimento = dataVencimento;
        Resgatado = false;
    }

    public void Resgatar()
    {
        if (Resgatado)
            throw new InvalidOperationException("Investimento já foi resgatado");

        Resgatado = true;
        DataResgate = DateTime.UtcNow;
        MarcarComoAtualizado();
    }
}
