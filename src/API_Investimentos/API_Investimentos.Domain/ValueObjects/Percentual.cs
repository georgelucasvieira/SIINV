namespace API_Investimentos.Domain.ValueObjects;

/// <summary>
/// Value Object para representar percentuais/taxas
/// </summary>
public sealed class Percentual : IEquatable<Percentual>, IComparable<Percentual>
{
    /// <summary>
    /// Valor da taxa em formato decimal (ex: 0.12 = 12%)
    /// </summary>
    public decimal Valor { get; }

    private Percentual(decimal valor)
    {
        Valor = valor;
    }

    /// <summary>
    /// Cria um percentual a partir de um valor decimal (ex: 0.12 para 12%)
    /// </summary>
    public static Percentual Criar(decimal valorDecimal)
    {
        if (valorDecimal < -1)
            throw new ArgumentException("Percentual não pode ser menor que -100%", nameof(valorDecimal));

        if (valorDecimal > 100)
            throw new ArgumentException("Percentual não pode ser maior que 10000%", nameof(valorDecimal));

        return new Percentual(Math.Round(valorDecimal, 6));
    }

    /// <summary>
    /// Cria um percentual a partir de um valor inteiro (ex: 12 para 12%)
    /// </summary>
    public static Percentual CriarDePercentual(decimal percentual)
    {
        return Criar(percentual / 100m);
    }

    /// <summary>
    /// Cria um percentual sem validação (usar apenas em cenários controlados)
    /// </summary>
    public static Percentual CriarSemValidacao(decimal valorDecimal) => new Percentual(valorDecimal);

    public static Percentual Zero => new Percentual(0);

    /// <summary>
    /// Retorna o valor em formato percentual (ex: 0.12 => 12)
    /// </summary>
    public decimal EmPercentual => Valor * 100m;

    /// <summary>
    /// Aplica o percentual a um valor monetário
    /// </summary>
    public Dinheiro AplicarA(Dinheiro valor) => Dinheiro.Criar(valor.Valor * Valor);


    public static Percentual operator +(Percentual a, Percentual b) => new Percentual(a.Valor + b.Valor);
    public static Percentual operator -(Percentual a, Percentual b) => new Percentual(a.Valor - b.Valor);
    public static Percentual operator *(Percentual a, decimal multiplicador) => new Percentual(a.Valor * multiplicador);
    public static bool operator >(Percentual a, Percentual b) => a.Valor > b.Valor;
    public static bool operator <(Percentual a, Percentual b) => a.Valor < b.Valor;
    public static bool operator >=(Percentual a, Percentual b) => a.Valor >= b.Valor;
    public static bool operator <=(Percentual a, Percentual b) => a.Valor <= b.Valor;


    public static implicit operator decimal(Percentual percentual) => percentual.Valor;


    public bool Equals(Percentual? other)
    {
        if (other is null) return false;
        return Valor == other.Valor;
    }

    public override bool Equals(object? obj) => obj is Percentual other && Equals(other);

    public override int GetHashCode() => Valor.GetHashCode();

    public static bool operator ==(Percentual? left, Percentual? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Percentual? left, Percentual? right) => !(left == right);


    public int CompareTo(Percentual? other)
    {
        if (other is null) return 1;
        return Valor.CompareTo(other.Valor);
    }

    public override string ToString() => $"{EmPercentual:N2}%";

    public string ToString(string format) => $"{EmPercentual.ToString(format)}%";
}
