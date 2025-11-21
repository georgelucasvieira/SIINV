using API_Investimentos.Application.Queries.Cliente;
using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace API_Investimentos.UnitTests.Application.Queries;

public class ObterClientePorIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepositorioCliente _repositorioCliente;
    private readonly ObterClientePorIdQueryHandler _handler;

    public ObterClientePorIdQueryHandlerTests()
    {
        _repositorioCliente = Substitute.For<IRepositorioCliente>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Clientes.Returns(_repositorioCliente);
        _handler = new ObterClientePorIdQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ComClienteExistente_DeveRetornarCliente()
    {
        var cliente = new Cliente("João Silva", "12345678901", "11999999999");
        var idProperty = typeof(Cliente).GetProperty("Id");
        idProperty?.SetValue(cliente, 1L);

        _repositorioCliente.ObterPorIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(cliente);

        var query = new ObterClientePorIdQuery { Id = 1 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.Nome.Should().Be("João Silva");
        resultado.Dados.Cpf.Should().Be("12345678901");
        resultado.Dados.Telefone.Should().Be("11999999999");
    }

    [Fact]
    public async Task Handle_ComClienteInexistente_DeveRetornarFalha()
    {
        _repositorioCliente.ObterPorIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Cliente?)null);

        var query = new ObterClientePorIdQuery { Id = 999 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_DeveMapearCorretamenteOsDados()
    {
        var cliente = new Cliente("Maria Santos", "98765432101", "11888888888");
        var idProperty = typeof(Cliente).GetProperty("Id");
        idProperty?.SetValue(cliente, 5L);

        _repositorioCliente.ObterPorIdAsync(5, Arg.Any<CancellationToken>())
            .Returns(cliente);

        var query = new ObterClientePorIdQuery { Id = 5 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Dados!.Id.Should().Be(5);
        resultado.Dados.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
