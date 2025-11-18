using API_Investimentos.Domain.Common;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;

namespace API_Investimentos.Domain.Entities;

/// <summary>
/// Entidade que representa um produto de investimento
/// </summary>
public class Produto : BaseEntity
{
    /// <summary>
    /// Nome do produto (ex: "CDB Banco XYZ 2026")
    /// </summary>
    public string Nome { get; private set; }

    /// <summary>
    /// Descrição detalhada do produto
    /// </summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Tipo do produto (CDB, Tesouro, LCI, etc.)
    /// </summary>
    public TipoProduto Tipo { get; private set; }

    /// <summary>
    /// Nível de risco do produto
    /// </summary>
    public NivelRisco NivelRisco { get; private set; }

    /// <summary>
    /// Taxa de rentabilidade anual
    /// </summary>
    public Percentual TaxaRentabilidade { get; private set; }

    /// <summary>
    /// Valor mínimo de investimento
    /// </summary>
    public Dinheiro ValorMinimo { get; private set; }

    /// <summary>
    /// Prazo mínimo em meses
    /// </summary>
    public int PrazoMinimoMeses { get; private set; }

    /// <summary>
    /// Prazo máximo em meses (null = sem limite)
    /// </summary>
    public int? PrazoMaximoMeses { get; private set; }

    /// <summary>
    /// Indica se o produto tem liquidez diária
    /// </summary>
    public bool LiquidezDiaria { get; private set; }

    /// <summary>
    /// Indica se o produto está ativo
    /// </summary>
    public bool Ativo { get; private set; }

    /// <summary>
    /// Taxa de administração (para fundos)
    /// </summary>
    public Percentual? TaxaAdministracao { get; private set; }

    /// <summary>
    /// Taxa de performance (para fundos)
    /// </summary>
    public Percentual? TaxaPerformance { get; private set; }

    /// <summary>
    /// Indica se o produto é isento de IR
    /// </summary>
    public bool IsentoIR { get; private set; }

    // Construtor privado para EF Core
    private Produto()
    {
        Nome = string.Empty;
        TaxaRentabilidade = Percentual.Zero;
        ValorMinimo = Dinheiro.Zero;
    }

    public Produto(
        string nome,
        TipoProduto tipo,
        NivelRisco nivelRisco,
        Percentual taxaRentabilidade,
        Dinheiro valorMinimo,
        int prazoMinimoMeses,
        bool liquidezDiaria,
        bool isentoIR)
    {
        ValidarNome(nome);
        ValidarPrazoMinimo(prazoMinimoMeses);

        Nome = nome;
        Tipo = tipo;
        NivelRisco = nivelRisco;
        TaxaRentabilidade = taxaRentabilidade;
        ValorMinimo = valorMinimo;
        PrazoMinimoMeses = prazoMinimoMeses;
        LiquidezDiaria = liquidezDiaria;
        IsentoIR = isentoIR;
        Ativo = true;
    }

    public void Atualizar(
        string nome,
        string? descricao,
        NivelRisco nivelRisco,
        Percentual taxaRentabilidade,
        Dinheiro valorMinimo,
        int prazoMinimoMeses,
        int? prazoMaximoMeses,
        bool liquidezDiaria)
    {
        ValidarNome(nome);
        ValidarPrazoMinimo(prazoMinimoMeses);

        if (prazoMaximoMeses.HasValue && prazoMaximoMeses.Value < prazoMinimoMeses)
            throw new ArgumentException("Prazo máximo não pode ser menor que o prazo mínimo");

        Nome = nome;
        Descricao = descricao;
        NivelRisco = nivelRisco;
        TaxaRentabilidade = taxaRentabilidade;
        ValorMinimo = valorMinimo;
        PrazoMinimoMeses = prazoMinimoMeses;
        PrazoMaximoMeses = prazoMaximoMeses;
        LiquidezDiaria = liquidezDiaria;

        MarcarComoAtualizado();
    }

    public void DefinirTaxaAdministracao(Percentual taxa)
    {
        if (Tipo != TipoProduto.Fundo)
            throw new InvalidOperationException("Taxa de administração só pode ser definida para fundos");

        TaxaAdministracao = taxa;
        MarcarComoAtualizado();
    }

    public void DefinirTaxaPerformance(Percentual taxa)
    {
        if (Tipo != TipoProduto.Fundo)
            throw new InvalidOperationException("Taxa de performance só pode ser definida para fundos");

        TaxaPerformance = taxa;
        MarcarComoAtualizado();
    }

    public void Ativar()
    {
        Ativo = true;
        MarcarComoAtualizado();
    }

    public void Desativar()
    {
        Ativo = false;
        MarcarComoAtualizado();
    }

    public bool PodeInvestir(Dinheiro valor, int prazoMeses)
    {
        if (!Ativo) return false;
        if (valor < ValorMinimo) return false;
        if (prazoMeses < PrazoMinimoMeses) return false;
        if (PrazoMaximoMeses.HasValue && prazoMeses > PrazoMaximoMeses.Value) return false;

        return true;
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório", nameof(nome));

        if (nome.Length > 200)
            throw new ArgumentException("Nome do produto não pode ter mais de 200 caracteres", nameof(nome));
    }

    private static void ValidarPrazoMinimo(int prazoMeses)
    {
        if (prazoMeses < 1)
            throw new ArgumentException("Prazo mínimo deve ser de pelo menos 1 mês", nameof(prazoMeses));

        if (prazoMeses > 600)
            throw new ArgumentException("Prazo mínimo não pode ser maior que 600 meses", nameof(prazoMeses));
    }
}
