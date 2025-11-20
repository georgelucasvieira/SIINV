using System.Net;
using System.Net.Http.Json;
using API_Investimentos.Application.DTOs.Requests;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.IntegrationTests.Controllers;

public class SimulacoesControllerTests : BaseIntegrationTest
{
    [Fact]
    public async Task SimularInvestimento_ComDadosValidos_DeveRetornarSimulacao()
    {
        // Arrange
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = 10000m,
            PrazoMeses = 12,
            TipoProduto = "CDB"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content.ReadFromJsonAsync<SimulacaoResponse>();
        resultado.Should().NotBeNull();
        resultado!.ProdutoValidado.Should().BeTrue();
        resultado.Produto.Should().NotBeNull();
        resultado.ResultadoSimulacao.Should().NotBeNull();
        resultado.ResultadoSimulacao.ValorInvestido.Should().Be(request.Valor);
        resultado.ResultadoSimulacao.ValorFinalBruto.Should().BeGreaterThan(request.Valor);
    }

    [Fact]
    public async Task SimularInvestimento_ComValorNegativo_DeveRetornarBadRequest()
    {
        // Arrange
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = -1000m,
            PrazoMeses = 12,
            TipoProduto = "CDB"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SimularInvestimento_ComPrazoZero_DeveRetornarBadRequest()
    {
        // Arrange
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = 10000m,
            PrazoMeses = 0,
            TipoProduto = "CDB"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SimularInvestimento_DeveCalcularImpostoCorretamente()
    {
        // Arrange
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = 10000m,
            PrazoMeses = 12,
            TipoProduto = "CDB"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);
        var resultado = await response.Content.ReadFromJsonAsync<SimulacaoResponse>();

        // Assert
        resultado.Should().NotBeNull();
        resultado!.ResultadoSimulacao.ValorIR.Should().BeGreaterThanOrEqualTo(0);
        resultado.ResultadoSimulacao.ValorFinalLiquido.Should().Be(
            resultado.ResultadoSimulacao.ValorFinalBruto - resultado.ResultadoSimulacao.ValorIR
        );
    }

    [Theory]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(36)]
    public async Task SimularInvestimento_ComDiferentesPrazos_DeveCalcularCorretamente(int prazoMeses)
    {
        // Arrange
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = 10000m,
            PrazoMeses = prazoMeses,
            TipoProduto = "CDB"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content.ReadFromJsonAsync<SimulacaoResponse>();
        resultado.Should().NotBeNull();
        resultado!.ResultadoSimulacao.PrazoMeses.Should().Be(prazoMeses);
    }

    [Theory]
    [InlineData("CDB")]
    [InlineData("LCI")]
    [InlineData("LCA")]
    [InlineData("TesouroSelic")]
    public async Task SimularInvestimento_ComDiferentesTiposProduto_DeveRetornarSimulacao(string tipoProduto)
    {
        // Arrange
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = 10000m,
            PrazoMeses = 12,
            TipoProduto = tipoProduto
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content.ReadFromJsonAsync<SimulacaoResponse>();
        resultado.Should().NotBeNull();
        resultado!.ProdutoValidado.Should().BeTrue();
        resultado.Produto.Should().NotBeNull();
        resultado.Produto!.Tipo.Should().Be(tipoProduto);
    }

    [Fact]
    public async Task SimularInvestimento_ProdutoIsentoIR_DeveRetornarImpostoZero()
    {
        // Arrange - LCI é isento de IR
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = 10000m,
            PrazoMeses = 12,
            TipoProduto = "LCI"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);
        var resultado = await response.Content.ReadFromJsonAsync<SimulacaoResponse>();

        // Assert
        resultado.Should().NotBeNull();
        resultado!.ResultadoSimulacao.ValorIR.Should().Be(0);
        resultado.ResultadoSimulacao.ValorFinalLiquido.Should().Be(resultado.ResultadoSimulacao.ValorFinalBruto);
    }

    [Fact]
    public async Task SimularInvestimento_DevePersistirSimulacaoNoBanco()
    {
        // Arrange
        var request = new SimularInvestimentoRequest
        {
            ClienteId = 1,
            Valor = 10000m,
            PrazoMeses = 12,
            TipoProduto = "CDB"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/simulacoes/simular", request);
        var resultado = await response.Content.ReadFromJsonAsync<SimulacaoResponse>();

        // Assert
        using var dbContext = GetDbContext();
        var simulacaoNoBanco = await dbContext.Simulacoes.FindAsync(resultado!.Id);

        simulacaoNoBanco.Should().NotBeNull();
        simulacaoNoBanco!.ClienteId.Should().Be(request.ClienteId);
        simulacaoNoBanco.ValorInvestido.Valor.Should().Be(request.Valor);
    }
}
