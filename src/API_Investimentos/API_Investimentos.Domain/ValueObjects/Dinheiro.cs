namespace API_Investimentos.Domain.ValueObjects;

/// <summary>
/// Value Object para representar valores monetários
/// </summary>
public sealed class Dinheiro : IEquatable<Dinheiro>, IComparable<Dinheiro>
{
    public decimal Valor { get; }

    private Dinheiro(decimal valor)
    {
        Valor = valor;
    }

    /// <summary>
    /// Cria um objeto Dinheiro com validação
    /// </summary>
    public static Dinheiro Criar(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("Valor monetário não pode ser negativo", nameof(valor));

        if (valor > 999_999_999_999.99m)
            throw new ArgumentException("Valor monetário excede o limite máximo permitido", nameof(valor));

        return new Dinheiro(Math.Round(valor, 2));
    }

    /// <summary>
    /// Cria um objeto Dinheiro sem validação (usar apenas em cenários controlados)
    /// </summary>
    public static Dinheiro CriarSemValidacao(decimal valor) => new Dinheiro(valor);

    public static Dinheiro Zero => new Dinheiro(0);


    public static Dinheiro operator +(Dinheiro a, Dinheiro b) => new Dinheiro(a.Valor + b.Valor);
    public static Dinheiro operator -(Dinheiro a, Dinheiro b) => new Dinheiro(a.Valor - b.Valor);
    public static Dinheiro operator *(Dinheiro a, decimal multiplicador) => new Dinheiro(a.Valor * multiplicador);
    public static Dinheiro operator /(Dinheiro a, decimal divisor) => new Dinheiro(a.Valor / divisor);
    public static bool operator >(Dinheiro a, Dinheiro b) => a.Valor > b.Valor;
    public static bool operator <(Dinheiro a, Dinheiro b) => a.Valor < b.Valor;
    public static bool operator >=(Dinheiro a, Dinheiro b) => a.Valor >= b.Valor;
    public static bool operator <=(Dinheiro a, Dinheiro b) => a.Valor <= b.Valor;


    public static implicit operator decimal(Dinheiro dinheiro) => dinheiro.Valor;


    public bool Equals(Dinheiro? other)
    {
        if (other is null) return false;
        return Valor == other.Valor;
    }

    public override bool Equals(object? obj) => obj is Dinheiro other && Equals(other);

    public override int GetHashCode() => Valor.GetHashCode();

    public static bool operator ==(Dinheiro? left, Dinheiro? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Dinheiro? left, Dinheiro? right) => !(left == right);


    public int CompareTo(Dinheiro? other)
    {
        if (other is null) return 1;
        return Valor.CompareTo(other.Valor);
    }

    public override string ToString() => Valor.ToString("N2");

    public string ToString(string format) => Valor.ToString(format);
}
