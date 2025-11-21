using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.UnitTests.Domain.Entities;

public class ProdutoTests
{
    [Fact]
    public void Construtor_ComParametrosValidos_DeveCriarProduto()
    {

        var produto = new Produto(
            nome: "CDB Teste",
            tipo: TipoProduto.CDB,
            nivelRisco: NivelRisco.Baixo,
            taxaRentabilidade: Percentual.CriarDePercentual(12m),
            valorMinimo: Dinheiro.Criar(1000m),
            prazoMinimoMeses: 6,
            liquidezDiaria: false,
            isentoIR: false
        );


        produto.Nome.Should().Be("CDB Teste");
        produto.Tipo.Should().Be(TipoProduto.CDB);
        produto.NivelRisco.Should().Be(NivelRisco.Baixo);
        produto.TaxaRentabilidade.EmPercentual.Should().Be(12m);
        produto.ValorMinimo.Valor.Should().Be(1000m);
        produto.PrazoMinimoMeses.Should().Be(6);
        produto.LiquidezDiaria.Should().BeFalse();
        produto.IsentoIR.Should().BeFalse();
        produto.Ativo.Should().BeTrue();
    }

    [Fact]
    public void PodeInvestir_ComValorAcimaDoMinimo_DeveRetornarTrue()
    {

        var produto = CriarProdutoTeste(valorMinimo: Dinheiro.Criar(1000m));
        var valorInvestimento = Dinheiro.Criar(1500m);
        var prazoMeses = 12;


        var resultado = produto.PodeInvestir(valorInvestimento, prazoMeses);


        resultado.Should().BeTrue();
    }

    [Fact]
    public void PodeInvestir_ComValorIgualAoMinimo_DeveRetornarTrue()
    {

        var produto = CriarProdutoTeste(valorMinimo: Dinheiro.Criar(1000m));
        var valorInvestimento = Dinheiro.Criar(1000m);
        var prazoMeses = 12;


        var resultado = produto.PodeInvestir(valorInvestimento, prazoMeses);


        resultado.Should().BeTrue();
    }

    [Fact]
    public void PodeInvestir_ComValorAbaixoDoMinimo_DeveRetornarFalse()
    {

        var produto = CriarProdutoTeste(valorMinimo: Dinheiro.Criar(1000m));
        var valorInvestimento = Dinheiro.Criar(500m);
        var prazoMeses = 12;


        var resultado = produto.PodeInvestir(valorInvestimento, prazoMeses);


        resultado.Should().BeFalse();
    }

    [Fact]
    public void PodeInvestir_ComProdutoInativo_DeveRetornarFalse()
    {

        var produto = CriarProdutoTeste(valorMinimo: Dinheiro.Criar(1000m));
        produto.Desativar();
        var valorInvestimento = Dinheiro.Criar(1500m);
        var prazoMeses = 12;


        var resultado = produto.PodeInvestir(valorInvestimento, prazoMeses);


        resultado.Should().BeFalse();
    }

    [Fact]
    public void DefinirTaxaAdministracao_ComValorValido_DeveDefinirTaxa()
    {

        var produto = CriarProdutoTeste(tipo: TipoProduto.Fundo);
        var taxa = Percentual.CriarDePercentual(2m);


        produto.DefinirTaxaAdministracao(taxa);


        produto.TaxaAdministracao.Should().Be(taxa);
    }

    [Fact]
    public void DefinirTaxaPerformance_ComValorValido_DeveDefinirTaxa()
    {

        var produto = CriarProdutoTeste(tipo: TipoProduto.Fundo);
        var taxa = Percentual.CriarDePercentual(20m);


        produto.DefinirTaxaPerformance(taxa);


        produto.TaxaPerformance.Should().Be(taxa);
    }

    [Fact]
    public void Desativar_DeveMarcarProdutoComoInativo()
    {

        var produto = CriarProdutoTeste();
        produto.Ativo.Should().BeTrue();


        produto.Desativar();


        produto.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Ativar_DeveMarcarProdutoComoAtivo()
    {

        var produto = CriarProdutoTeste();
        produto.Desativar();
        produto.Ativo.Should().BeFalse();


        produto.Ativar();


        produto.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData(TipoProduto.CDB)]
    [InlineData(TipoProduto.TesouroSelic)]
    [InlineData(TipoProduto.LCI)]
    [InlineData(TipoProduto.LCA)]
    public void Construtor_ComDiferentesTipos_DeveCriarProdutoCorretamente(TipoProduto tipo)
    {

        var produto = new Produto(
            nome: $"Produto {tipo}",
            tipo: tipo,
            nivelRisco: NivelRisco.Baixo,
            taxaRentabilidade: Percentual.CriarDePercentual(10m),
            valorMinimo: Dinheiro.Criar(100m),
            prazoMinimoMeses: 1,
            liquidezDiaria: true,
            isentoIR: false
        );


        produto.Tipo.Should().Be(tipo);
    }

    [Theory]
    [InlineData(NivelRisco.MuitoBaixo)]
    [InlineData(NivelRisco.Baixo)]
    [InlineData(NivelRisco.Medio)]
    [InlineData(NivelRisco.Alto)]
    [InlineData(NivelRisco.MuitoAlto)]
    public void Construtor_ComDiferentesNiveisRisco_DeveCriarProdutoCorretamente(NivelRisco nivelRisco)
    {

        var produto = CriarProdutoTeste(nivelRisco: nivelRisco);


        produto.NivelRisco.Should().Be(nivelRisco);
    }

    [Fact]
    public void ProdutoLCI_DeveSerIsentoIR()
    {

        var produto = new Produto(
            nome: "LCI Teste",
            tipo: TipoProduto.LCI,
            nivelRisco: NivelRisco.Baixo,
            taxaRentabilidade: Percentual.CriarDePercentual(10m),
            valorMinimo: Dinheiro.Criar(10000m),
            prazoMinimoMeses: 12,
            liquidezDiaria: false,
            isentoIR: true
        );


        produto.IsentoIR.Should().BeTrue();
    }

    [Fact]
    public void ProdutoLCA_DeveSerIsentoIR()
    {

        var produto = new Produto(
            nome: "LCA Teste",
            tipo: TipoProduto.LCA,
            nivelRisco: NivelRisco.Baixo,
            taxaRentabilidade: Percentual.CriarDePercentual(10m),
            valorMinimo: Dinheiro.Criar(10000m),
            prazoMinimoMeses: 12,
            liquidezDiaria: false,
            isentoIR: true
        );


        produto.IsentoIR.Should().BeTrue();
    }

    private static Produto CriarProdutoTeste(
        string nome = "Produto Teste",
        TipoProduto tipo = TipoProduto.CDB,
        NivelRisco nivelRisco = NivelRisco.Baixo,
        Percentual? taxaRentabilidade = null,
        Dinheiro? valorMinimo = null,
        int prazoMinimoMeses = 6,
        bool liquidezDiaria = false,
        bool isentoIR = false)
    {
        return new Produto(
            nome: nome,
            tipo: tipo,
            nivelRisco: nivelRisco,
            taxaRentabilidade: taxaRentabilidade ?? Percentual.CriarDePercentual(12m),
            valorMinimo: valorMinimo ?? Dinheiro.Criar(1000m),
            prazoMinimoMeses: prazoMinimoMeses,
            liquidezDiaria: liquidezDiaria,
            isentoIR: isentoIR
        );
    }
}
