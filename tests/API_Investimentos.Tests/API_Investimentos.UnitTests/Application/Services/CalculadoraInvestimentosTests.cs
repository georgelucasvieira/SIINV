using API_Investimentos.Application.Services;
using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;

namespace API_Investimentos.UnitTests.Application.Services;

public class CalculadoraInvestimentosTests
{
    private readonly CalculadoraInvestimentos _calculadora;

    public CalculadoraInvestimentosTests()
    {
        _calculadora = new CalculadoraInvestimentos();
    }

    [Fact]
    public void Calcular_CDB_DeveCalcularCorretamente()
    {
        var produto = CriarProduto(TipoProduto.CDB, 10m, false);
        var valor = Dinheiro.Criar(10000);
        var prazo = 12;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorFinalBruto.Valor.Should().BeGreaterThan(10000);
        resultado.ValorIR.Valor.Should().BeGreaterThan(0);
        resultado.ValorFinalLiquido.Valor.Should().BeLessThan(resultado.ValorFinalBruto.Valor);
        resultado.AliquotaIR.Valor.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Calcular_LCI_DeveSerIsentoDeIR()
    {
        var produto = CriarProduto(TipoProduto.LCI, 8m, true);
        var valor = Dinheiro.Criar(10000);
        var prazo = 12;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorIR.Valor.Should().Be(0);
        resultado.AliquotaIR.Valor.Should().Be(0);
        resultado.ValorFinalLiquido.Should().Be(resultado.ValorFinalBruto);
    }

    [Fact]
    public void Calcular_LCA_DeveSerIsentoDeIR()
    {
        var produto = CriarProduto(TipoProduto.LCA, 8m, true);
        var valor = Dinheiro.Criar(10000);
        var prazo = 12;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorIR.Valor.Should().Be(0);
        resultado.AliquotaIR.Valor.Should().Be(0);
    }

    [Fact]
    public void Calcular_TesouroSelic_DeveCalcularCorretamente()
    {
        var produto = CriarProduto(TipoProduto.TesouroSelic, 11m, false);
        var valor = Dinheiro.Criar(5000);
        var prazo = 24;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorFinalBruto.Valor.Should().BeGreaterThan(5000);
        resultado.DataVencimento.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void Calcular_TesouroPrefixado_DeveCalcularCorretamente()
    {
        var produto = CriarProduto(TipoProduto.TesouroPrefixado, 12m, false);
        var valor = Dinheiro.Criar(10000);
        var prazo = 36;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorFinalBruto.Valor.Should().BeGreaterThan(10000);
    }

    [Fact]
    public void Calcular_TesouroIPCA_DeveIncluirIPCA()
    {
        var produto = CriarProduto(TipoProduto.TesouroIPCA, 5m, false);
        var valor = Dinheiro.Criar(10000);
        var prazo = 60;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorFinalBruto.Valor.Should().BeGreaterThan(10000);
    }

    [Fact]
    public void Calcular_Fundo_ComTaxaAdmin_DeveDescontarTaxa()
    {
        var produto = CriarProdutoFundo(10m, 1m, null);
        var valor = Dinheiro.Criar(10000);
        var prazo = 12;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorFinalBruto.Valor.Should().BeGreaterThan(10000);
    }

    [Fact]
    public void Calcular_Fundo_ComTaxaPerformance_DeveDescontarTaxa()
    {
        var produto = CriarProdutoFundo(10m, 1m, 20m);
        var valor = Dinheiro.Criar(10000);
        var prazo = 12;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        resultado.ValorFinalBruto.Valor.Should().BeGreaterThan(10000);
    }

    [Theory]
    [InlineData(3, 22.5)]
    [InlineData(6, 22.5)]
    [InlineData(9, 20.0)]
    [InlineData(12, 20.0)]
    [InlineData(18, 17.5)]
    [InlineData(24, 17.5)]
    [InlineData(30, 15.0)]
    [InlineData(36, 15.0)]
    public void Calcular_AliquotaIR_DeveSerRegressiva(int prazoMeses, decimal aliquotaEsperada)
    {
        var produto = CriarProduto(TipoProduto.CDB, 10m, false);
        var valor = Dinheiro.Criar(10000);

        var resultado = _calculadora.Calcular(produto, valor, prazoMeses);

        var aliquotaPercentual = resultado.AliquotaIR.Valor * 100;
        aliquotaPercentual.Should().Be(aliquotaEsperada);
    }

    [Fact]
    public void Calcular_DataVencimento_DeveEstarCorreta()
    {
        var produto = CriarProduto(TipoProduto.CDB, 10m, false);
        var valor = Dinheiro.Criar(10000);
        var prazo = 12;

        var resultado = _calculadora.Calcular(produto, valor, prazo);

        var dataEsperada = DateTime.UtcNow.AddMonths(12);
        resultado.DataVencimento.Date.Should().Be(dataEsperada.Date);
    }

    private Produto CriarProduto(TipoProduto tipo, decimal rentabilidade, bool isentoIR)
    {
        return new Produto(
            $"Produto {tipo}",
            tipo,
            NivelRisco.Medio,
            Percentual.CriarDePercentual(rentabilidade),
            Dinheiro.Criar(100),
            1,
            false,
            isentoIR
        );
    }

    private Produto CriarProdutoFundo(decimal rentabilidade, decimal taxaAdmin, decimal? taxaPerformance)
    {
        var produto = new Produto(
            "Fundo Teste",
            TipoProduto.Fundo,
            NivelRisco.Alto,
            Percentual.CriarDePercentual(rentabilidade),
            Dinheiro.Criar(1000),
            12,
            false,
            false
        );
        produto.DefinirTaxaAdministracao(Percentual.CriarDePercentual(taxaAdmin));
        if (taxaPerformance.HasValue)
        {
            produto.DefinirTaxaPerformance(Percentual.CriarDePercentual(taxaPerformance.Value));
        }
        return produto;
    }
}
