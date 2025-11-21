using System.Reflection;
using API_Investimentos.Application.Commands.Simulacao;
using API_Investimentos.Application.Messaging.Interfaces;
using API_Investimentos.Application.Services;
using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace API_Investimentos.UnitTests.Application.Handlers;

public class SimularInvestimentoCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICalculadoraInvestimentos _calculadora;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IRepositorioProduto _repositorioProduto;
    private readonly IRepositorioSimulacao _repositorioSimulacao;
    private readonly SimularInvestimentoCommandHandler _handler;

    public SimularInvestimentoCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _calculadora = new CalculadoraInvestimentos();
        _messagePublisher = Substitute.For<IMessagePublisher>();
        _repositorioProduto = Substitute.For<IRepositorioProduto>();
        _repositorioSimulacao = Substitute.For<IRepositorioSimulacao>();

        _unitOfWork.Produtos.Returns(_repositorioProduto);
        _unitOfWork.Simulacoes.Returns(_repositorioSimulacao);

        _handler = new SimularInvestimentoCommandHandler(
            _unitOfWork,
            _calculadora,
            _messagePublisher);
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveRetornarSucesso()
    {
        var command = CriarCommandValido();
        var produto = CriarProdutoValido();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.ResultadoSimulacao.ValorInvestido.Should().Be(command.Valor);
    }

    [Fact]
    public async Task Handle_ComTipoProdutoInvalido_DeveRetornarFalha()
    {
        var command = CriarCommandValido();
        command.TipoProduto = "TipoInvalido";

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("Tipo de produto inválido");
    }

    [Fact]
    public async Task Handle_SemProdutosDisponiveis_DeveRetornarFalha()
    {
        var command = CriarCommandValido();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto>());

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("Nenhum produto do tipo");
    }

    [Fact]
    public async Task Handle_ComProdutoInativo_DeveRetornarFalha()
    {
        var command = CriarCommandValido();
        var produto = CriarProdutoValido();
        produto.Desativar();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("Nenhum produto do tipo");
    }

    [Fact]
    public async Task Handle_ComValorAbaixoDoMinimo_DeveRetornarFalha()
    {
        var command = CriarCommandValido();
        command.Valor = 50;

        var produto = CriarProdutoValido();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("Nenhum produto adequado");
    }

    [Fact]
    public async Task Handle_DeveSalvarSimulacao()
    {
        var command = CriarCommandValido();
        var produto = CriarProdutoValido();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        await _handler.Handle(command, CancellationToken.None);

        await _repositorioSimulacao.Received(1)
            .AdicionarAsync(Arg.Any<Simulacao>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DevePublicarEvento()
    {
        var command = CriarCommandValido();
        var produto = CriarProdutoValido();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        await _handler.Handle(command, CancellationToken.None);

        await _messagePublisher.Received(1)
            .PublishToExchangeAsync(
                Arg.Any<object>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveRetornarProdutoValidado()
    {
        var command = CriarCommandValido();
        var produto = CriarProdutoValido();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Dados!.ProdutoValidado.Should().BeTrue();
        resultado.Dados.Produto.Should().NotBeNull();
        resultado.Dados.Produto!.Nome.Should().Be(produto.Nome);
    }

    [Fact]
    public async Task Handle_DeveCalcularRendimentos()
    {
        var command = CriarCommandValido();
        var produto = CriarProdutoValido();

        _repositorioProduto
            .ObterPorTipoAsync(Arg.Any<TipoProduto>(), Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        var resultado = await _handler.Handle(command, CancellationToken.None);

        var simulacao = resultado.Dados!.ResultadoSimulacao;
        simulacao.ValorFinalBruto.Should().BeGreaterThan(command.Valor);
        simulacao.RendimentoBruto.Should().BeGreaterThan(0);
        simulacao.RendimentoLiquido.Should().BeGreaterThan(0);
    }

    private SimularInvestimentoCommand CriarCommandValido()
    {
        return new SimularInvestimentoCommand
        {
            ClienteId = 1,
            Valor = 10000,
            PrazoMeses = 12,
            TipoProduto = "CDB"
        };
    }

    private Produto CriarProdutoValido()
    {
        var produto = new Produto(
            "CDB Banco XYZ",
            TipoProduto.CDB,
            NivelRisco.Medio,
            Percentual.CriarDePercentual(10m),
            Dinheiro.Criar(100),
            1,
            false,
            false
        );

        var idProperty = typeof(Produto).GetProperty("Id");
        idProperty?.SetValue(produto, 1L);

        return produto;
    }
}
