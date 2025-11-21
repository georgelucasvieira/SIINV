using API_Investimentos.Application.Commands.Cliente;
using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace API_Investimentos.UnitTests.Application.Handlers;

public class CadastrarClienteCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepositorioCliente _repositorioCliente;
    private readonly CadastrarClienteCommandHandler _handler;

    public CadastrarClienteCommandHandlerTests()
    {
        _repositorioCliente = Substitute.For<IRepositorioCliente>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Clientes.Returns(_repositorioCliente);
        _handler = new CadastrarClienteCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveRetornarSucesso()
    {
        var command = new CadastrarClienteCommand
        {
            Nome = "João Silva",
            Cpf = "12345678901",
            Telefone = "11999999999"
        };

        _repositorioCliente.ExisteCpfAsync(command.Cpf, Arg.Any<CancellationToken>())
            .Returns(false);

        _repositorioCliente.AdicionarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Cliente>());

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.Nome.Should().Be(command.Nome);
        resultado.Dados.Cpf.Should().Be(command.Cpf);
        resultado.Dados.Telefone.Should().Be(command.Telefone);
        resultado.Mensagem.Should().Contain("sucesso");
    }

    [Fact]
    public async Task Handle_ComCpfExistente_DeveRetornarFalha()
    {
        var command = new CadastrarClienteCommand
        {
            Nome = "João Silva",
            Cpf = "12345678901",
            Telefone = "11999999999"
        };

        _repositorioCliente.ExisteCpfAsync(command.Cpf, Arg.Any<CancellationToken>())
            .Returns(true);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("CPF");
    }

    [Fact]
    public async Task Handle_DeveSalvarAlteracoes()
    {
        var command = new CadastrarClienteCommand
        {
            Nome = "Maria Santos",
            Cpf = "98765432101",
            Telefone = "11888888888"
        };

        _repositorioCliente.ExisteCpfAsync(command.Cpf, Arg.Any<CancellationToken>())
            .Returns(false);

        _repositorioCliente.AdicionarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Cliente>());

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
