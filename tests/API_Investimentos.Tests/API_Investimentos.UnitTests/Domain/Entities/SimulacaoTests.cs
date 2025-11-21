using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.UnitTests.Domain.Entities;

public class SimulacaoTests
{
    [Fact]
    public void Construtor_ComParametrosValidos_DeveCriarSimulacao()
    {

        var simulacao = CriarSimulacaoTeste();


        simulacao.ClienteId.Should().Be(1);
        simulacao.ProdutoId.Should().Be(1);
        simulacao.ValorInvestido.Valor.Should().Be(10000m);
        simulacao.PrazoMeses.Should().Be(12);
        simulacao.ValorFinalBruto.Valor.Should().Be(11200m);
        simulacao.ValorFinalLiquido.Valor.Should().Be(11000m);
        simulacao.Status.Should().Be(StatusSimulacao.Pendente);
    }

    [Fact]
    public void Construtor_ComClienteIdInvalido_DeveLancarExcecao()
    {

        Action act = () => new Simulacao(
            clienteId: 0,
            produtoId: 1,
            valorInvestido: Dinheiro.Criar(10000m),
            prazoMeses: 12,
            dataVencimento: DateTime.UtcNow.AddMonths(12),
            valorFinalBruto: Dinheiro.Criar(11200m),
            valorIR: Dinheiro.Criar(200m),
            valorFinalLiquido: Dinheiro.Criar(11000m),
            taxaRentabilidadeEfetiva: Percentual.CriarDePercentual(12m),
            aliquotaIR: Percentual.CriarDePercentual(20m)
        );


        act.Should().Throw<ArgumentException>()
            .WithMessage("ClienteId inválido*");
    }

    [Fact]
    public void Construtor_ComProdutoIdInvalido_DeveLancarExcecao()
    {

        Action act = () => new Simulacao(
            clienteId: 1,
            produtoId: 0,
            valorInvestido: Dinheiro.Criar(10000m),
            prazoMeses: 12,
            dataVencimento: DateTime.UtcNow.AddMonths(12),
            valorFinalBruto: Dinheiro.Criar(11200m),
            valorIR: Dinheiro.Criar(200m),
            valorFinalLiquido: Dinheiro.Criar(11000m),
            taxaRentabilidadeEfetiva: Percentual.CriarDePercentual(12m),
            aliquotaIR: Percentual.CriarDePercentual(20m)
        );


        act.Should().Throw<ArgumentException>()
            .WithMessage("ProdutoId inválido*");
    }

    [Fact]
    public void Construtor_ComPrazoInvalido_DeveLancarExcecao()
    {

        Action act = () => new Simulacao(
            clienteId: 1,
            produtoId: 1,
            valorInvestido: Dinheiro.Criar(10000m),
            prazoMeses: 0,
            dataVencimento: DateTime.UtcNow,
            valorFinalBruto: Dinheiro.Criar(11200m),
            valorIR: Dinheiro.Criar(200m),
            valorFinalLiquido: Dinheiro.Criar(11000m),
            taxaRentabilidadeEfetiva: Percentual.CriarDePercentual(12m),
            aliquotaIR: Percentual.CriarDePercentual(20m)
        );


        act.Should().Throw<ArgumentException>()
            .WithMessage("Prazo deve ser maior que zero*");
    }

    [Fact]
    public void MarcarComoConcluida_DeveAlterarStatusParaConcluida()
    {

        var simulacao = CriarSimulacaoTeste();


        simulacao.MarcarComoConcluida();


        simulacao.Status.Should().Be(StatusSimulacao.Concluida);
    }

    [Fact]
    public void MarcarComoErro_ComMensagem_DeveAlterarStatusParaErro()
    {

        var simulacao = CriarSimulacaoTeste();
        var mensagemErro = "Erro ao processar simulação";


        simulacao.MarcarComoErro(mensagemErro);


        simulacao.Status.Should().Be(StatusSimulacao.Erro);
        simulacao.Observacoes.Should().Be(mensagemErro);
    }

    [Fact]
    public void Cancelar_DeveAlterarStatusParaCancelada()
    {

        var simulacao = CriarSimulacaoTeste();


        simulacao.Cancelar();


        simulacao.Status.Should().Be(StatusSimulacao.Cancelada);
    }

    [Fact]
    public void Cancelar_ComMotivo_DeveRegistrarMotivo()
    {

        var simulacao = CriarSimulacaoTeste();
        var motivo = "Cancelada pelo usuário";


        simulacao.Cancelar(motivo);


        simulacao.Status.Should().Be(StatusSimulacao.Cancelada);
        simulacao.Observacoes.Should().Be(motivo);
    }

    [Fact]
    public void CalcularRendimentoBruto_DeveRetornarDiferencaEntreValorFinalEInvestido()
    {

        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(10000m),
            valorFinalBruto: Dinheiro.Criar(11200m)
        );


        var rendimento = simulacao.CalcularRendimentoBruto();


        rendimento.Valor.Should().Be(1200m);
    }

    [Fact]
    public void CalcularRendimentoLiquido_DeveRetornarDiferencaEntreValorFinalLiquidoEInvestido()
    {

        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(10000m),
            valorFinalLiquido: Dinheiro.Criar(11000m)
        );


        var rendimento = simulacao.CalcularRendimentoLiquido();


        rendimento.Valor.Should().Be(1000m);
    }

    [Fact]
    public void CalcularRentabilidadeLiquida_DeveCalcularPercentualCorreto()
    {

        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(10000m),
            valorFinalLiquido: Dinheiro.Criar(11000m)
        );


        var rentabilidade = simulacao.CalcularRentabilidadeLiquida();



        rentabilidade.EmPercentual.Should().BeApproximately(10m, 0.01m);
    }

    [Fact]
    public void CalcularRentabilidadeLiquida_ComValorInvestidoZero_DeveRetornarZero()
    {

        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(0m),
            valorFinalLiquido: Dinheiro.Criar(0m)
        );


        var rentabilidade = simulacao.CalcularRentabilidadeLiquida();


        rentabilidade.Should().Be(Percentual.Zero);
    }

    [Fact]
    public void DefinirUsuarioAtualizacao_DeveDefinirUsuarioId()
    {

        var simulacao = CriarSimulacaoTeste();
        var usuarioId = 5L;


        simulacao.DefinirUsuarioAtualizacao(usuarioId);


        simulacao.AtualizadoPorId.Should().Be(usuarioId);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(36)]
    public void Construtor_ComDiferentesPrazos_DeveCriarSimulacaoCorretamente(int prazoMeses)
    {

        var simulacao = CriarSimulacaoTeste(prazoMeses: prazoMeses);


        simulacao.PrazoMeses.Should().Be(prazoMeses);
    }

    [Fact]
    public void DataVencimento_DeveSerCalculadaCorretamente()
    {

        var dataAtual = DateTime.UtcNow;
        var prazoMeses = 12;
        var dataVencimentoEsperada = dataAtual.AddMonths(prazoMeses);


        var simulacao = CriarSimulacaoTeste(
            prazoMeses: prazoMeses,
            dataVencimento: dataVencimentoEsperada
        );


        simulacao.DataVencimento.Should().BeCloseTo(dataVencimentoEsperada, TimeSpan.FromSeconds(5));
    }

    private static Simulacao CriarSimulacaoTeste(
        long clienteId = 1,
        long produtoId = 1,
        Dinheiro? valorInvestido = null,
        int prazoMeses = 12,
        DateTime? dataVencimento = null,
        Dinheiro? valorFinalBruto = null,
        Dinheiro? valorIR = null,
        Dinheiro? valorFinalLiquido = null,
        Percentual? taxaRentabilidadeEfetiva = null,
        Percentual? aliquotaIR = null)
    {
        return new Simulacao(
            clienteId: clienteId,
            produtoId: produtoId,
            valorInvestido: valorInvestido ?? Dinheiro.Criar(10000m),
            prazoMeses: prazoMeses,
            dataVencimento: dataVencimento ?? DateTime.UtcNow.AddMonths(prazoMeses),
            valorFinalBruto: valorFinalBruto ?? Dinheiro.Criar(11200m),
            valorIR: valorIR ?? Dinheiro.Criar(200m),
            valorFinalLiquido: valorFinalLiquido ?? Dinheiro.Criar(11000m),
            taxaRentabilidadeEfetiva: taxaRentabilidadeEfetiva ?? Percentual.CriarDePercentual(12m),
            aliquotaIR: aliquotaIR ?? Percentual.CriarDePercentual(20m)
        );
    }
}
