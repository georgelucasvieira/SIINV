using API_Investimentos.Domain.Entities;
using FluentAssertions;

namespace API_Investimentos.UnitTests.Domain.Entities;

public class ClienteTests
{
    [Fact]
    public void Constructor_ComDadosValidos_DeveCriarCliente()
    {
        var cliente = new Cliente("João Silva", "12345678901", "11999999999");

        cliente.Nome.Should().Be("João Silva");
        cliente.Cpf.Should().Be("12345678901");
        cliente.Telefone.Should().Be("11999999999");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ComNomeVazio_DeveLancarExcecao(string nome)
    {
        Action act = () => new Cliente(nome, "12345678901", "11999999999");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Nome*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ComCpfVazio_DeveLancarExcecao(string cpf)
    {
        Action act = () => new Cliente("João Silva", cpf, "11999999999");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*CPF*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ComTelefoneVazio_DeveLancarExcecao(string telefone)
    {
        Action act = () => new Cliente("João Silva", "12345678901", telefone);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Telefone*");
    }

    [Fact]
    public void Constructor_DeveLimparCpf()
    {
        var cliente = new Cliente("João Silva", "123.456.789-01", "11999999999");

        cliente.Cpf.Should().Be("12345678901");
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarCliente()
    {
        var cliente = new Cliente("João Silva", "12345678901", "11999999999");

        cliente.Atualizar("Maria Santos", "11888888888");

        cliente.Nome.Should().Be("Maria Santos");
        cliente.Telefone.Should().Be("11888888888");
        cliente.Cpf.Should().Be("12345678901");
    }

    [Fact]
    public void Constructor_DeveCriarComTimestamps()
    {
        var antes = DateTime.UtcNow;
        var cliente = new Cliente("João Silva", "12345678901", "11999999999");
        var depois = DateTime.UtcNow;

        cliente.CriadoEm.Should().BeOnOrAfter(antes);
        cliente.CriadoEm.Should().BeOnOrBefore(depois);
    }
}
