using API_Investimentos.Application.Queries.PerfilRisco;
using API_Investimentos.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using PerfilRiscoEntity = API_Investimentos.Domain.Entities.PerfilRisco;

namespace API_Investimentos.UnitTests.Application.Queries;

public class ObterPerfilRiscoQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepositorioPerfilRisco _repositorioPerfilRisco;
    private readonly ObterPerfilRiscoQueryHandler _handler;

    public ObterPerfilRiscoQueryHandlerTests()
    {
        _repositorioPerfilRisco = Substitute.For<IRepositorioPerfilRisco>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.PerfisRisco.Returns(_repositorioPerfilRisco);
        _handler = new ObterPerfilRiscoQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ComPerfilExistente_DeveRetornarPerfil()
    {
        var perfil = new PerfilRiscoEntity(1, 75);

        _repositorioPerfilRisco.ObterPorClienteAsync(1, Arg.Any<CancellationToken>())
            .Returns(perfil);

        var query = new ObterPerfilRiscoQuery { ClienteId = 1 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.ClienteId.Should().Be(1);
        resultado.Dados.Pontuacao.Should().Be(75);
    }

    [Fact]
    public async Task Handle_ComPerfilInexistente_DeveRetornarFalha()
    {
        _repositorioPerfilRisco.ObterPorClienteAsync(999, Arg.Any<CancellationToken>())
            .Returns((PerfilRiscoEntity?)null);

        var query = new ObterPerfilRiscoQuery { ClienteId = 999 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("não encontrado");
    }

    [Fact]
    public async Task Handle_DeveMapearCorretamenteOPerfil()
    {
        var perfil = new PerfilRiscoEntity(5, 30);

        _repositorioPerfilRisco.ObterPorClienteAsync(5, Arg.Any<CancellationToken>())
            .Returns(perfil);

        var query = new ObterPerfilRiscoQuery { ClienteId = 5 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Dados!.Perfil.Should().NotBeNullOrEmpty();
        resultado.Dados.Descricao.Should().NotBeNullOrEmpty();
        resultado.Dados.ProximaAvaliacao.Should().BeAfter(DateTime.UtcNow);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(90)]
    public async Task Handle_ComDiferentesPontuacoes_DeveRetornarPerfilCorreto(int pontuacao)
    {
        var perfil = new PerfilRiscoEntity(1, pontuacao);

        _repositorioPerfilRisco.ObterPorClienteAsync(1, Arg.Any<CancellationToken>())
            .Returns(perfil);

        var query = new ObterPerfilRiscoQuery { ClienteId = 1 };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados!.Pontuacao.Should().Be(pontuacao);
    }
}
