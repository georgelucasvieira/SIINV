using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.UnitTests.Domain.ValueObjects;

public class DinheiroTests
{
    [Fact]
    public void Criar_ComValorValido_DeveCriarDinheiro()
    {

        var valor = 100.50m;


        var dinheiro = Dinheiro.Criar(valor);


        dinheiro.Valor.Should().Be(100.50m);
    }

    [Fact]
    public void Criar_ComValorNegativo_DeveLancarExcecao()
    {

        var valor = -10m;


        Action act = () => Dinheiro.Criar(valor);


        act.Should().Throw<ArgumentException>()
            .WithMessage("Valor monetário não pode ser negativo*");
    }

    [Fact]
    public void Criar_ComValorAcimaDoLimite_DeveLancarExcecao()
    {

        var valor = 1_000_000_000_000m;


        Action act = () => Dinheiro.Criar(valor);


        act.Should().Throw<ArgumentException>()
            .WithMessage("Valor monetário excede o limite máximo permitido*");
    }

    [Fact]
    public void Criar_ComValorZero_DeveCriarDinheiro()
    {

        var valor = 0m;


        var dinheiro = Dinheiro.Criar(valor);


        dinheiro.Valor.Should().Be(0m);
    }

    [Theory]
    [InlineData(100.123, 100.12)]
    [InlineData(100.999, 101.00)]
    [InlineData(100.125, 100.12)]
    public void Criar_DeveArredondarParaDuasCasasDecimais(decimal valorOriginal, decimal valorEsperado)
    {

        var dinheiro = Dinheiro.Criar(valorOriginal);


        dinheiro.Valor.Should().Be(valorEsperado);
    }

    [Fact]
    public void OperadorSoma_DeveRetornarSomaCorreta()
    {

        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(50m);


        var resultado = dinheiro1 + dinheiro2;


        resultado.Valor.Should().Be(150m);
    }

    [Fact]
    public void OperadorSubtracao_DeveRetornarSubtracaoCorreta()
    {

        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(30m);


        var resultado = dinheiro1 - dinheiro2;


        resultado.Valor.Should().Be(70m);
    }

    [Fact]
    public void OperadorMultiplicacao_DeveRetornarMultiplicacaoCorreta()
    {

        var dinheiro = Dinheiro.Criar(100m);
        var multiplicador = 1.5m;


        var resultado = dinheiro * multiplicador;


        resultado.Valor.Should().Be(150m);
    }

    [Fact]
    public void OperadorDivisao_DeveRetornarDivisaoCorreta()
    {

        var dinheiro = Dinheiro.Criar(100m);
        var divisor = 2m;


        var resultado = dinheiro / divisor;


        resultado.Valor.Should().Be(50m);
    }

    [Fact]
    public void OperadorDivisao_ComDivisorZero_DeveLancarExcecao()
    {

        var dinheiro = Dinheiro.Criar(100m);


        Action act = () => _ = dinheiro / 0m;


        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void OperadorMaiorQue_DeveCompararCorretamente()
    {

        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(50m);


        (dinheiro1 > dinheiro2).Should().BeTrue();
        (dinheiro2 > dinheiro1).Should().BeFalse();
    }

    [Fact]
    public void OperadorMenorQue_DeveCompararCorretamente()
    {

        var dinheiro1 = Dinheiro.Criar(50m);
        var dinheiro2 = Dinheiro.Criar(100m);


        (dinheiro1 < dinheiro2).Should().BeTrue();
        (dinheiro2 < dinheiro1).Should().BeFalse();
    }

    [Fact]
    public void OperadorIgualdade_DeveCompararCorretamente()
    {

        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(100m);
        var dinheiro3 = Dinheiro.Criar(50m);


        (dinheiro1 == dinheiro2).Should().BeTrue();
        (dinheiro1 == dinheiro3).Should().BeFalse();
    }

    [Fact]
    public void Equals_ComObjetoIgual_DeveRetornarTrue()
    {

        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(100m);


        dinheiro1.Equals(dinheiro2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveFormatarCorretamente()
    {

        var dinheiro = Dinheiro.Criar(1234.56m);


        var resultado = dinheiro.ToString();


        resultado.Should().Be("1.234,56");
    }

    [Fact]
    public void GetHashCode_ParaValoresIguais_DeveRetornarHashCodeIgual()
    {

        var dinheiro1 = Dinheiro.Criar(100m);
        var dinheiro2 = Dinheiro.Criar(100m);


        dinheiro1.GetHashCode().Should().Be(dinheiro2.GetHashCode());
    }
}
