using API_Investimentos.Domain.Enums;
using FluentAssertions;
using PerfilRiscoEntity = API_Investimentos.Domain.Entities.PerfilRisco;

namespace API_Investimentos.UnitTests.Domain.Entities;

public class PerfilRiscoTests
{
    [Fact]
    public void Constructor_ComPontuacaoValida_DeveCriarPerfil()
    {
        var perfil = new PerfilRiscoEntity(1, 50);

        perfil.ClienteId.Should().Be(1);
        perfil.Pontuacao.Should().Be(50);
    }

    [Theory]
    [InlineData(0, PerfilInvestidor.Conservador)]
    [InlineData(25, PerfilInvestidor.Conservador)]
    [InlineData(35, PerfilInvestidor.Conservador)]
    [InlineData(36, PerfilInvestidor.Moderado)]
    [InlineData(50, PerfilInvestidor.Moderado)]
    [InlineData(65, PerfilInvestidor.Moderado)]
    [InlineData(66, PerfilInvestidor.Agressivo)]
    [InlineData(80, PerfilInvestidor.Agressivo)]
    [InlineData(100, PerfilInvestidor.Agressivo)]
    public void Constructor_ComPontuacao_DeveDefinirPerfilCorreto(int pontuacao, PerfilInvestidor esperado)
    {
        var perfil = new PerfilRiscoEntity(1, pontuacao);

        perfil.Perfil.Should().Be(esperado);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_ComPontuacaoInvalida_DeveLancarExcecao(int pontuacao)
    {
        Action act = () => new PerfilRiscoEntity(1, pontuacao);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Pontuação*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ComClienteIdInvalido_DeveLancarExcecao(long clienteId)
    {
        Action act = () => new PerfilRiscoEntity(clienteId, 50);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*ClienteId*");
    }

    [Fact]
    public void Constructor_DeveDefinirDescricaoAutomaticamente()
    {
        var perfil = new PerfilRiscoEntity(1, 50);

        perfil.Descricao.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_DeveDefinirDataProximaAvaliacao()
    {
        var perfil = new PerfilRiscoEntity(1, 50);

        perfil.DataProximaAvaliacao.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_ComFatoresCalculo_DeveArmazenar()
    {
        var fatores = "{\"teste\": 123}";
        var perfil = new PerfilRiscoEntity(1, 50, fatores);

        perfil.FatoresCalculo.Should().Be(fatores);
    }
}
