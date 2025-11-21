using API_Investimentos.Application.Commands.Simulacao;
using API_Investimentos.Application.Validators;
using FluentAssertions;

namespace API_Investimentos.UnitTests.Application.Validators;

public class SimularInvestimentoCommandValidatorTests
{
    private readonly SimularInvestimentoCommandValidator _validator;

    public SimularInvestimentoCommandValidatorTests()
    {
        _validator = new SimularInvestimentoCommandValidator();
    }

    [Fact]
    public void Validar_ComDadosValidos_DeveSerValido()
    {
        var command = CriarCommandValido();

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validar_ComClienteIdInvalido_DeveSerInvalido(long clienteId)
    {
        var command = CriarCommandValido();
        command.ClienteId = clienteId;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "ClienteId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Validar_ComValorInvalido_DeveSerInvalido(decimal valor)
    {
        var command = CriarCommandValido();
        command.Valor = valor;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Valor");
    }

    [Fact]
    public void Validar_ComValorAcimaDoLimite_DeveSerInvalido()
    {
        var command = CriarCommandValido();
        command.Valor = 1_000_000_000_000m;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Valor");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-12)]
    public void Validar_ComPrazoInvalido_DeveSerInvalido(int prazo)
    {
        var command = CriarCommandValido();
        command.PrazoMeses = prazo;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "PrazoMeses");
    }

    [Fact]
    public void Validar_ComPrazoAcimaDoLimite_DeveSerInvalido()
    {
        var command = CriarCommandValido();
        command.PrazoMeses = 601;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "PrazoMeses");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validar_ComTipoProdutoVazio_DeveSerInvalido(string tipoProduto)
    {
        var command = CriarCommandValido();
        command.TipoProduto = tipoProduto!;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "TipoProduto");
    }

    [Theory]
    [InlineData("Invalido")]
    [InlineData("ABC")]
    [InlineData("Poupanca")]
    public void Validar_ComTipoProdutoInvalido_DeveSerInvalido(string tipoProduto)
    {
        var command = CriarCommandValido();
        command.TipoProduto = tipoProduto;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "TipoProduto");
    }

    [Theory]
    [InlineData("CDB")]
    [InlineData("cdb")]
    [InlineData("TesouroSelic")]
    [InlineData("TesouroPrefixado")]
    [InlineData("TesouroIPCA")]
    [InlineData("LCI")]
    [InlineData("LCA")]
    [InlineData("Fundo")]
    public void Validar_ComTipoProdutoValido_DeveSerValido(string tipoProduto)
    {
        var command = CriarCommandValido();
        command.TipoProduto = tipoProduto;

        var resultado = _validator.Validate(command);

        resultado.IsValid.Should().BeTrue();
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
}
