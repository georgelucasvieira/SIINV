using API_Investimentos.Domain.Common;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;

namespace API_Investimentos.Domain.Entities;

/// <summary>
/// Entidade que representa uma simulação de investimento
/// </summary>
public class Simulacao : BaseEntity, IAuditavel
{
    /// <summary>
    /// ID do cliente que realizou a simulação
    /// </summary>
    public long ClienteId { get; private set; }

    /// <summary>
    /// ID do produto simulado
    /// </summary>
    public long ProdutoId { get; private set; }

    /// <summary>
    /// Produto relacionado (navegação)
    /// </summary>
    public Produto? Produto { get; private set; }

    /// <summary>
    /// Valor investido
    /// </summary>
    public Dinheiro ValorInvestido { get; private set; }

    /// <summary>
    /// Prazo do investimento em meses
    /// </summary>
    public int PrazoMeses { get; private set; }

    /// <summary>
    /// Data de vencimento calculada
    /// </summary>
    public DateTime DataVencimento { get; private set; }

    /// <summary>
    /// Valor final calculado (bruto)
    /// </summary>
    public Dinheiro ValorFinalBruto { get; private set; }

    /// <summary>
    /// Valor do Imposto de Renda
    /// </summary>
    public Dinheiro ValorIR { get; private set; }

    /// <summary>
    /// Valor final líquido (após IR)
    /// </summary>
    public Dinheiro ValorFinalLiquido { get; private set; }

    /// <summary>
    /// Taxa de rentabilidade efetiva aplicada
    /// </summary>
    public Percentual TaxaRentabilidadeEfetiva { get; private set; }

    /// <summary>
    /// Alíquota de IR aplicada
    /// </summary>
    public Percentual AliquotaIR { get; private set; }

    /// <summary>
    /// Status da simulação
    /// </summary>
    public StatusSimulacao Status { get; private set; }

    /// <summary>
    /// Observações ou mensagens de erro
    /// </summary>
    public string? Observacoes { get; private set; }

    /// <summary>
    /// ID do usuário que criou o registro
    /// </summary>
    public long? CriadoPorId { get; private set; }

    /// <summary>
    /// ID do usuário que atualizou o registro
    /// </summary>
    public long? AtualizadoPorId { get; private set; }


    private Simulacao()
    {
        ValorInvestido = Dinheiro.Zero;
        ValorFinalBruto = Dinheiro.Zero;
        ValorIR = Dinheiro.Zero;
        ValorFinalLiquido = Dinheiro.Zero;
        TaxaRentabilidadeEfetiva = Percentual.Zero;
        AliquotaIR = Percentual.Zero;
    }

    public Simulacao(
        long clienteId,
        long produtoId,
        Dinheiro valorInvestido,
        int prazoMeses,
        DateTime dataVencimento,
        Dinheiro valorFinalBruto,
        Dinheiro valorIR,
        Dinheiro valorFinalLiquido,
        Percentual taxaRentabilidadeEfetiva,
        Percentual aliquotaIR,
        long? criadoPorId = null)
    {
        if (clienteId <= 0)
            throw new ArgumentException("ClienteId inválido", nameof(clienteId));

        if (produtoId <= 0)
            throw new ArgumentException("ProdutoId inválido", nameof(produtoId));

        if (prazoMeses <= 0)
            throw new ArgumentException("Prazo deve ser maior que zero", nameof(prazoMeses));

        ClienteId = clienteId;
        ProdutoId = produtoId;
        ValorInvestido = valorInvestido;
        PrazoMeses = prazoMeses;
        DataVencimento = dataVencimento;
        ValorFinalBruto = valorFinalBruto;
        ValorIR = valorIR;
        ValorFinalLiquido = valorFinalLiquido;
        TaxaRentabilidadeEfetiva = taxaRentabilidadeEfetiva;
        AliquotaIR = aliquotaIR;
        Status = StatusSimulacao.Pendente;
        CriadoPorId = criadoPorId;
    }

    public void MarcarComoConcluida()
    {
        Status = StatusSimulacao.Concluida;
        MarcarComoAtualizado();
    }

    public void MarcarComoErro(string mensagemErro)
    {
        Status = StatusSimulacao.Erro;
        Observacoes = mensagemErro;
        MarcarComoAtualizado();
    }

    public void Cancelar(string? motivo = null)
    {
        Status = StatusSimulacao.Cancelada;
        Observacoes = motivo;
        MarcarComoAtualizado();
    }

    public void DefinirUsuarioAtualizacao(long usuarioId)
    {
        AtualizadoPorId = usuarioId;
        MarcarComoAtualizado();
    }

    /// <summary>
    /// Calcula o rendimento bruto
    /// </summary>
    public Dinheiro CalcularRendimentoBruto() => ValorFinalBruto - ValorInvestido;

    /// <summary>
    /// Calcula o rendimento líquido
    /// </summary>
    public Dinheiro CalcularRendimentoLiquido() => ValorFinalLiquido - ValorInvestido;

    /// <summary>
    /// Calcula a rentabilidade percentual líquida
    /// </summary>
    public Percentual CalcularRentabilidadeLiquida()
    {
        if (ValorInvestido.Valor == 0) return Percentual.Zero;

        var rendimento = CalcularRendimentoLiquido();
        return Percentual.Criar(rendimento.Valor / ValorInvestido.Valor);
    }
}
