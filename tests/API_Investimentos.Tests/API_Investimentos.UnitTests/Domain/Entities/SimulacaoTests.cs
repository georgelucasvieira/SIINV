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
        // Arrange & Act
        var simulacao = CriarSimulacaoTeste();

        // Assert
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
        // Arrange & Act
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

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("ClienteId inválido*");
    }

    [Fact]
    public void Construtor_ComProdutoIdInvalido_DeveLancarExcecao()
    {
        // Arrange & Act
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

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("ProdutoId inválido*");
    }

    [Fact]
    public void Construtor_ComPrazoInvalido_DeveLancarExcecao()
    {
        // Arrange & Act
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

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Prazo deve ser maior que zero*");
    }

    [Fact]
    public void MarcarComoConcluida_DeveAlterarStatusParaConcluida()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste();

        // Act
        simulacao.MarcarComoConcluida();

        // Assert
        simulacao.Status.Should().Be(StatusSimulacao.Concluida);
    }

    [Fact]
    public void MarcarComoErro_ComMensagem_DeveAlterarStatusParaErro()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste();
        var mensagemErro = "Erro ao processar simulação";

        // Act
        simulacao.MarcarComoErro(mensagemErro);

        // Assert
        simulacao.Status.Should().Be(StatusSimulacao.Erro);
        simulacao.Observacoes.Should().Be(mensagemErro);
    }

    [Fact]
    public void Cancelar_DeveAlterarStatusParaCancelada()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste();

        // Act
        simulacao.Cancelar();

        // Assert
        simulacao.Status.Should().Be(StatusSimulacao.Cancelada);
    }

    [Fact]
    public void Cancelar_ComMotivo_DeveRegistrarMotivo()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste();
        var motivo = "Cancelada pelo usuário";

        // Act
        simulacao.Cancelar(motivo);

        // Assert
        simulacao.Status.Should().Be(StatusSimulacao.Cancelada);
        simulacao.Observacoes.Should().Be(motivo);
    }

    [Fact]
    public void CalcularRendimentoBruto_DeveRetornarDiferencaEntreValorFinalEInvestido()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(10000m),
            valorFinalBruto: Dinheiro.Criar(11200m)
        );

        // Act
        var rendimento = simulacao.CalcularRendimentoBruto();

        // Assert
        rendimento.Valor.Should().Be(1200m);
    }

    [Fact]
    public void CalcularRendimentoLiquido_DeveRetornarDiferencaEntreValorFinalLiquidoEInvestido()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(10000m),
            valorFinalLiquido: Dinheiro.Criar(11000m)
        );

        // Act
        var rendimento = simulacao.CalcularRendimentoLiquido();

        // Assert
        rendimento.Valor.Should().Be(1000m);
    }

    [Fact]
    public void CalcularRentabilidadeLiquida_DeveCalcularPercentualCorreto()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(10000m),
            valorFinalLiquido: Dinheiro.Criar(11000m)
        );

        // Act
        var rentabilidade = simulacao.CalcularRentabilidadeLiquida();

        // Assert
        // (11000 - 10000) / 10000 = 0.1 = 10%
        rentabilidade.Percentual.Should().BeApproximately(10m, 0.01m);
    }

    [Fact]
    public void CalcularRentabilidadeLiquida_ComValorInvestidoZero_DeveRetornarZero()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste(
            valorInvestido: Dinheiro.Criar(0m),
            valorFinalLiquido: Dinheiro.Criar(0m)
        );

        // Act
        var rentabilidade = simulacao.CalcularRentabilidadeLiquida();

        // Assert
        rentabilidade.Should().Be(Percentual.Zero);
    }

    [Fact]
    public void DefinirUsuarioAtualizacao_DeveDefinirUsuarioId()
    {
        // Arrange
        var simulacao = CriarSimulacaoTeste();
        var usuarioId = 5L;

        // Act
        simulacao.DefinirUsuarioAtualizacao(usuarioId);

        // Assert
        simulacao.AtualizadoPorId.Should().Be(usuarioId);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(36)]
    public void Construtor_ComDiferentesPrazos_DeveCriarSimulacaoCorretamente(int prazoMeses)
    {
        // Act
        var simulacao = CriarSimulacaoTeste(prazoMeses: prazoMeses);

        // Assert
        simulacao.PrazoMeses.Should().Be(prazoMeses);
    }

    [Fact]
    public void DataVencimento_DeveSerCalculadaCorretamente()
    {
        // Arrange
        var dataAtual = DateTime.UtcNow;
        var prazoMeses = 12;
        var dataVencimentoEsperada = dataAtual.AddMonths(prazoMeses);

        // Act
        var simulacao = CriarSimulacaoTeste(
            prazoMeses: prazoMeses,
            dataVencimento: dataVencimentoEsperada
        );

        // Assert
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
