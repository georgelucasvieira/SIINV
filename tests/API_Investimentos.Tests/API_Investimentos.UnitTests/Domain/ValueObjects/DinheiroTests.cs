using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.UnitTests.Domain.ValueObjects;

public class DinheiroTests
{
    [Fact]
    public void Criar_ComValorValido_DeveCriarDinheiro()
    {
        // Arrange
        var valor = 100.50m;

        // Act
        var dinheiro = Dinheiro.Criar(valor);

        // Assert
        dinheiro.Valor.Should().Be(100.50m);
    }

    [Fact]
    public void Criar_ComValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        var valor = -10m;

        // Act
        Action act = () => Dinheiro.Criar(valor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Valor monetário não pode ser negativo*");
    }

    [Fact]
    public void Criar_ComValorAcimaDoLimite_DeveLancarExcecao()
    {
        // Arrange
        var valor = 1_000_000_000_000m;

        // Act
        Action act = () => Dinheiro.Criar(valor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Valor monetário excede o limite máximo permitido*");
    }

    [Fact]
    public void Criar_ComValorZero_DeveCriarDinheiro()
    {
        // Arrange
        var valor = 0m;

        // Act
        var dinheiro = Dinheiro.Criar(valor);

        // Assert
        dinheiro.Valor.Should().Be(0m);
    }

    [Theory]
    [InlineData(100.123, 100.12)]
    [InlineData(100.999, 101.00)]
    [InlineData(100.125, 100.13)]
    public void Criar_DeveArredondarParaDuasCasasDecimais(decimal valorOriginal, decimal valorEsperado)
    {
        // Act
        var dinheiro = Dinheiro.Criar(valorOriginal);

        // Assert
        dinheiro.Valor.Should().Be(valorEsperado);
    }

    [Fact]
    public void OperadorSoma_DeveRetornarSomaCorreta()
    {
        // Arrange
        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(50m);

        // Act
        var resultado = dinheiro1 + dinheiro2;

        // Assert
        resultado.Valor.Should().Be(150m);
    }

    [Fact]
    public void OperadorSubtracao_DeveRetornarSubtracaoCorreta()
    {
        // Arrange
        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(30m);

        // Act
        var resultado = dinheiro1 - dinheiro2;

        // Assert
        resultado.Valor.Should().Be(70m);
    }

    [Fact]
    public void OperadorMultiplicacao_DeveRetornarMultiplicacaoCorreta()
    {
        // Arrange
        var dinheiro = Dinheiro.Criar(100m);
        var multiplicador = 1.5m;

        // Act
        var resultado = dinheiro * multiplicador;

        // Assert
        resultado.Valor.Should().Be(150m);
    }

    [Fact]
    public void OperadorDivisao_DeveRetornarDivisaoCorreta()
    {
        // Arrange
        var dinheiro = Dinheiro.Criar(100m);
        var divisor = 2m;

        // Act
        var resultado = dinheiro / divisor;

        // Assert
        resultado.Valor.Should().Be(50m);
    }

    [Fact]
    public void OperadorDivisao_ComDivisorZero_DeveLancarExcecao()
    {
        // Arrange
        var dinheiro = Dinheiro.Criar(100m);

        // Act
        Action act = () => _ = dinheiro / 0m;

        // Assert
        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void OperadorMaiorQue_DeveCompararCorretamente()
    {
        // Arrange
        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(50m);

        // Act & Assert
        (dinheiro1 > dinheiro2).Should().BeTrue();
        (dinheiro2 > dinheiro1).Should().BeFalse();
    }

    [Fact]
    public void OperadorMenorQue_DeveCompararCorretamente()
    {
        // Arrange
        var dinheiro1 = Dinheiro.Criar(50m);
        var dinheiro2 = Dinheiro.Criar(100m);

        // Act & Assert
        (dinheiro1 < dinheiro2).Should().BeTrue();
        (dinheiro2 < dinheiro1).Should().BeFalse();
    }

    [Fact]
    public void OperadorIgualdade_DeveCompararCorretamente()
    {
        // Arrange
        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(100m);
        var dinheiro3 = Dinheiro.Criar(50m);

        // Act & Assert
        (dinheiro1 == dinheiro2).Should().BeTrue();
        (dinheiro1 == dinheiro3).Should().BeFalse();
    }

    [Fact]
    public void Equals_ComObjetoIgual_DeveRetornarTrue()
    {
        // Arrange
        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(100m);

        // Act & Assert
        dinheiro1.Equals(dinheiro2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveFormatarCorretamente()
    {
        // Arrange
        var dinheiro = Dinheiro.Criar(1234.56m);

        // Act
        var resultado = dinheiro.ToString();

        // Assert
        resultado.Should().Be("R$ 1.234,56");
    }

    [Fact]
    public void GetHashCode_ParaValoresIguais_DeveRetornarHashCodeIgual()
    {
        // Arrange
        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(100m);

        // Act & Assert
        dinheiro1.GetHashCode().Should().Be(dinheiro2.GetHashCode());
    }
}
