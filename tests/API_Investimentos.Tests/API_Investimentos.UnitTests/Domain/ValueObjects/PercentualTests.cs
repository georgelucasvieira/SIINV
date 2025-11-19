using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.UnitTests.Domain.ValueObjects;

public class PercentualTests
{
    [Fact]
    public void CriarDePercentual_ComValorValido_DeveCriarPercentual()
    {
        // Arrange
        var percentual = 12.5m;

        // Act
        var resultado = Percentual.CriarDePercentual(percentual);

        // Assert
        resultado.Valor.Should().Be(0.125m);
        resultado.EmPercentual.Should().Be(12.5m);
    }

    [Fact]
    public void CriarDePercentual_ComValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        var percentual = -150m; // Menor que -100%

        // Act
        Action act = () => Percentual.CriarDePercentual(percentual);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CriarDePercentual_ComValorMuitoAlto_DeveLancarExcecao()
    {
        // Arrange
        var percentual = 15000m; // Maior que 10000%

        // Act
        Action act = () => Percentual.CriarDePercentual(percentual);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CriarDePercentual_ComZero_DeveCriarPercentual()
    {
        // Arrange
        var percentual = 0m;

        // Act
        var resultado = Percentual.CriarDePercentual(percentual);

        // Assert
        resultado.Valor.Should().Be(0m);
        resultado.EmPercentual.Should().Be(0m);
    }

    [Fact]
    public void CriarDePercentual_Com100_DeveCriarPercentual()
    {
        // Arrange
        var percentual = 100m;

        // Act
        var resultado = Percentual.CriarDePercentual(percentual);

        // Assert
        resultado.Valor.Should().Be(1m);
        resultado.EmPercentual.Should().Be(100m);
    }

    [Theory]
    [InlineData(10, 0.1)]
    [InlineData(25, 0.25)]
    [InlineData(50, 0.5)]
    [InlineData(75, 0.75)]
    public void CriarDePercentual_DeveConverterCorretamente(decimal percentual, decimal valorEsperado)
    {
        // Act
        var resultado = Percentual.CriarDePercentual(percentual);

        // Assert
        resultado.Valor.Should().Be(valorEsperado);
    }

    [Fact]
    public void OperadorSoma_DeveRetornarSomaCorreta()
    {
        // Arrange
        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(5m);

        // Act
        var resultado = percentual1 + percentual2;

        // Assert
        resultado.EmPercentual.Should().Be(15m);
    }

    [Fact]
    public void OperadorSubtracao_DeveRetornarSubtracaoCorreta()
    {
        // Arrange
        var percentual1 = Percentual.CriarDePercentual(15m);
        var percentual2 = Percentual.CriarDePercentual(5m);

        // Act
        var resultado = percentual1 - percentual2;

        // Assert
        resultado.EmPercentual.Should().Be(10m);
    }

    [Fact]
    public void OperadorMultiplicacao_DeveRetornarMultiplicacaoCorreta()
    {
        // Arrange
        var percentual = Percentual.CriarDePercentual(10m);
        var multiplicador = 2m;

        // Act
        var resultado = percentual * multiplicador;

        // Assert
        resultado.EmPercentual.Should().Be(20m);
    }

    [Fact]
    public void OperadorDivisao_DeveRetornarDivisaoCorreta()
    {
        // Arrange
        var percentual = Percentual.CriarDePercentual(20m);
        var divisor = 2m;

        // Act
        var resultado = percentual / divisor;

        // Assert
        resultado.EmPercentual.Should().Be(10m);
    }

    [Fact]
    public void OperadorMaiorQue_DeveCompararCorretamente()
    {
        // Arrange
        var percentual1 = Percentual.CriarDePercentual(15m);
        var percentual2 = Percentual.CriarDePercentual(10m);

        // Act & Assert
        (percentual1 > percentual2).Should().BeTrue();
        (percentual2 > percentual1).Should().BeFalse();
    }

    [Fact]
    public void OperadorMenorQue_DeveCompararCorretamente()
    {
        // Arrange
        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(15m);

        // Act & Assert
        (percentual1 < percentual2).Should().BeTrue();
        (percentual2 < percentual1).Should().BeFalse();
    }

    [Fact]
    public void OperadorIgualdade_DeveCompararCorretamente()
    {
        // Arrange
        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(10m);
        var percentual3 = Percentual.CriarDePercentual(15m);

        // Act & Assert
        (percentual1 == percentual2).Should().BeTrue();
        (percentual1 == percentual3).Should().BeFalse();
    }

    [Fact]
    public void Equals_ComObjetoIgual_DeveRetornarTrue()
    {
        // Arrange
        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(10m);

        // Act & Assert
        percentual1.Equals(percentual2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveFormatarCorretamente()
    {
        // Arrange
        var percentual = Percentual.CriarDePercentual(12.5m);

        // Act
        var resultado = percentual.ToString();

        // Assert
        resultado.Should().Be("12,50%");
    }

    [Fact]
    public void GetHashCode_ParaValoresIguais_DeveRetornarHashCodeIgual()
    {
        // Arrange
        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(10m);

        // Act & Assert
        percentual1.GetHashCode().Should().Be(percentual2.GetHashCode());
    }

    [Theory]
    [InlineData(12.123456, 12.12)]
    [InlineData(12.999999, 13.00)]
    [InlineData(12.125, 12.13)]
    public void CriarDePercentual_DeveArredondarParaDuasCasasDecimais(decimal percentual, decimal percentualEsperado)
    {
        // Act
        var resultado = Percentual.CriarDePercentual(percentual);

        // Assert
        resultado.EmPercentual.Should().BeApproximately(percentualEsperado, 0.01m);
    }

    [Fact]
    public void Zero_DeveRetornarPercentualZero()
    {
        // Act
        var zero = Percentual.Zero;

        // Assert
        zero.Valor.Should().Be(0m);
        zero.EmPercentual.Should().Be(0m);
    }

    [Fact]
    public void Criar_ComValorDecimal_DeveCriarCorretamente()
    {
        // Arrange
        var valorDecimal = 0.15m; // 15%

        // Act
        var percentual = Percentual.Criar(valorDecimal);

        // Assert
        percentual.Valor.Should().Be(0.15m);
        percentual.EmPercentual.Should().Be(15m);
    }
}
