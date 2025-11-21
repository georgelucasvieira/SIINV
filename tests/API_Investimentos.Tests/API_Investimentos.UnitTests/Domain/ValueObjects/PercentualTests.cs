using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.UnitTests.Domain.ValueObjects;

public class PercentualTests
{
    [Fact]
    public void CriarDePercentual_ComValorValido_DeveCriarPercentual()
    {

        var percentual = 12.5m;


        var resultado = Percentual.CriarDePercentual(percentual);


        resultado.Valor.Should().Be(0.125m);
        resultado.EmPercentual.Should().Be(12.5m);
    }

    [Fact]
    public void CriarDePercentual_ComValorNegativo_DeveLancarExcecao()
    {

        var percentual = -150m; // Menor que -100%


        Action act = () => Percentual.CriarDePercentual(percentual);


        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CriarDePercentual_ComValorMuitoAlto_DeveLancarExcecao()
    {

        var percentual = 15000m; // Maior que 10000%


        Action act = () => Percentual.CriarDePercentual(percentual);


        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CriarDePercentual_ComZero_DeveCriarPercentual()
    {

        var percentual = 0m;


        var resultado = Percentual.CriarDePercentual(percentual);


        resultado.Valor.Should().Be(0m);
        resultado.EmPercentual.Should().Be(0m);
    }

    [Fact]
    public void CriarDePercentual_Com100_DeveCriarPercentual()
    {

        var percentual = 100m;


        var resultado = Percentual.CriarDePercentual(percentual);


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

        var resultado = Percentual.CriarDePercentual(percentual);


        resultado.Valor.Should().Be(valorEsperado);
    }

    [Fact]
    public void OperadorSoma_DeveRetornarSomaCorreta()
    {

        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(5m);


        var resultado = percentual1 + percentual2;


        resultado.EmPercentual.Should().Be(15m);
    }

    [Fact]
    public void OperadorSubtracao_DeveRetornarSubtracaoCorreta()
    {

        var percentual1 = Percentual.CriarDePercentual(15m);
        var percentual2 = Percentual.CriarDePercentual(5m);


        var resultado = percentual1 - percentual2;


        resultado.EmPercentual.Should().Be(10m);
    }

    [Fact]
    public void OperadorMultiplicacao_DeveRetornarMultiplicacaoCorreta()
    {

        var percentual = Percentual.CriarDePercentual(10m);
        var multiplicador = 2m;


        var resultado = percentual * multiplicador;


        resultado.EmPercentual.Should().Be(20m);
    }

    [Fact]
    public void OperadorMaiorQue_DeveCompararCorretamente()
    {

        var percentual1 = Percentual.CriarDePercentual(15m);
        var percentual2 = Percentual.CriarDePercentual(10m);


        (percentual1 > percentual2).Should().BeTrue();
        (percentual2 > percentual1).Should().BeFalse();
    }

    [Fact]
    public void OperadorMenorQue_DeveCompararCorretamente()
    {

        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(15m);


        (percentual1 < percentual2).Should().BeTrue();
        (percentual2 < percentual1).Should().BeFalse();
    }

    [Fact]
    public void OperadorIgualdade_DeveCompararCorretamente()
    {

        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(10m);
        var percentual3 = Percentual.CriarDePercentual(15m);


        (percentual1 == percentual2).Should().BeTrue();
        (percentual1 == percentual3).Should().BeFalse();
    }

    [Fact]
    public void Equals_ComObjetoIgual_DeveRetornarTrue()
    {

        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(10m);


        percentual1.Equals(percentual2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveFormatarCorretamente()
    {

        var percentual = Percentual.CriarDePercentual(12.5m);


        var resultado = percentual.ToString();


        resultado.Should().Be("12,50%");
    }

    [Fact]
    public void GetHashCode_ParaValoresIguais_DeveRetornarHashCodeIgual()
    {

        var percentual1 = Percentual.CriarDePercentual(10m);
        var percentual2 = Percentual.CriarDePercentual(10m);


        percentual1.GetHashCode().Should().Be(percentual2.GetHashCode());
    }

    [Theory]
    [InlineData(12.123456, 12.12)]
    [InlineData(12.999999, 13.00)]
    [InlineData(12.125, 12.12)]
    public void CriarDePercentual_DeveArredondarParaDuasCasasDecimais(decimal percentual, decimal percentualEsperado)
    {

        var resultado = Percentual.CriarDePercentual(percentual);


        resultado.EmPercentual.Should().BeApproximately(percentualEsperado, 0.01m);
    }

    [Fact]
    public void Zero_DeveRetornarPercentualZero()
    {

        var zero = Percentual.Zero;


        zero.Valor.Should().Be(0m);
        zero.EmPercentual.Should().Be(0m);
    }

    [Fact]
    public void Criar_ComValorDecimal_DeveCriarCorretamente()
    {

        var valorDecimal = 0.15m; // 15%


        var percentual = Percentual.Criar(valorDecimal);


        percentual.Valor.Should().Be(0.15m);
        percentual.EmPercentual.Should().Be(15m);
    }
}
