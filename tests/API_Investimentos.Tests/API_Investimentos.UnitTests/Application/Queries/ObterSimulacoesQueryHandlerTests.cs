using System.Reflection;
using API_Investimentos.Application.Queries.Simulacao;
using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace API_Investimentos.UnitTests.Application.Queries;

public class ObterSimulacoesQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepositorioSimulacao _repositorioSimulacao;
    private readonly ObterSimulacoesQueryHandler _handler;

    public ObterSimulacoesQueryHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _repositorioSimulacao = Substitute.For<IRepositorioSimulacao>();
        _unitOfWork.Simulacoes.Returns(_repositorioSimulacao);
        _handler = new ObterSimulacoesQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_SemFiltro_DeveRetornarTodasSimulacoes()
    {
        var simulacoes = CriarListaSimulacoes();
        _repositorioSimulacao.ObterTodosAsync(Arg.Any<CancellationToken>())
            .Returns(simulacoes);

        var query = new ObterSimulacoesQuery();

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ComClienteId_DeveFiltrarPorCliente()
    {
        var simulacoes = new List<Simulacao> { CriarSimulacao(1) };
        _repositorioSimulacao.ObterPorClienteAsync(1, Arg.Any<CancellationToken>())
            .Returns(simulacoes);

        var query = new ObterSimulacoesQuery { ClienteId = 1 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        await _repositorioSimulacao.Received(1).ObterPorClienteAsync(1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComPeriodo_DeveFiltrarPorPeriodo()
    {
        var dataInicio = DateTime.UtcNow.AddDays(-30);
        var dataFim = DateTime.UtcNow;
        var simulacoes = CriarListaSimulacoes();

        _repositorioSimulacao.ObterPorPeriodoAsync(dataInicio, dataFim, Arg.Any<CancellationToken>())
            .Returns(simulacoes);

        var query = new ObterSimulacoesQuery { DataInicio = dataInicio, DataFim = dataFim };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        await _repositorioSimulacao.Received(1)
            .ObterPorPeriodoAsync(dataInicio, dataFim, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveMapearSimulacaoCorretamente()
    {
        var simulacao = CriarSimulacao(1);
        _repositorioSimulacao.ObterTodosAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Simulacao> { simulacao });

        var query = new ObterSimulacoesQuery();

        var resultado = await _handler.Handle(query, CancellationToken.None);

        var response = resultado.Dados![0];
        response.ProdutoValidado.Should().BeTrue();
        response.ResultadoSimulacao.ValorInvestido.Should().Be(10000);
        response.ResultadoSimulacao.PrazoMeses.Should().Be(12);
    }

    private List<Simulacao> CriarListaSimulacoes()
    {
        return new List<Simulacao>
        {
            CriarSimulacao(1),
            CriarSimulacao(2)
        };
    }

    private Simulacao CriarSimulacao(long clienteId)
    {
        var simulacao = new Simulacao(
            clienteId,
            1,
            Dinheiro.Criar(10000),
            12,
            DateTime.UtcNow.AddMonths(12),
            Dinheiro.Criar(11000),
            Dinheiro.Criar(200),
            Dinheiro.Criar(10800),
            Percentual.CriarDePercentual(10),
            Percentual.CriarDePercentual(20)
        );

        var idProperty = typeof(Simulacao).GetProperty("Id");
        idProperty?.SetValue(simulacao, clienteId);

        return simulacao;
    }
}
