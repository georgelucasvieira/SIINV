using System.Net;
using System.Net.Http.Json;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace API_Investimentos.IntegrationTests.Controllers;

public class ProdutosControllerTests : BaseIntegrationTest
{
    [Fact]
    public async Task ListarProdutos_DeveRetornarListaDeProdutos()
    {

        var response = await Client.GetAsync("/api/v1/produtos");


        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();
        produtos.Should().NotBeNull();
        produtos.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ListarProdutos_DeveRetornarProdutosComPropriedadesCorretas()
    {

        var response = await Client.GetAsync("/api/v1/produtos");
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();


        produtos.Should().NotBeNull();
        produtos.Should().AllSatisfy(p =>
        {
            p.Id.Should().BeGreaterThan(0);
            p.Nome.Should().NotBeNullOrEmpty();
            p.Tipo.Should().NotBeNullOrEmpty();
            p.Risco.Should().NotBeNullOrEmpty();
            p.Rentabilidade.Should().BeGreaterThanOrEqualTo(0);
            p.ValorMinimo.Should().BeGreaterThanOrEqualTo(0);
            p.PrazoMinimoMeses.Should().BeGreaterThanOrEqualTo(0);
            p.Ativo.Should().BeTrue();
        });
    }

    [Fact]
    public async Task ListarProdutos_ComTipoFiltro_DeveRetornarApenasProdutosDoTipo()
    {

        var tipo = "CDB";


        var response = await Client.GetAsync($"/api/v1/produtos?tipo={tipo}");
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        produtos.Should().NotBeNull();
        produtos.Should().AllSatisfy(p => p.Tipo.Should().Be(tipo));
    }

    [Fact]
    public async Task ListarProdutos_ComRiscoFiltro_DeveRetornarApenasProdutosDoRisco()
    {

        var risco = "Baixo";


        var response = await Client.GetAsync($"/api/v1/produtos?risco={risco}");
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        produtos.Should().NotBeNull();
        produtos.Should().AllSatisfy(p => p.Risco.Should().Be(risco));
    }

    [Fact]
    public async Task ListarProdutos_ComApenasAtivos_DeveRetornarApenasProdutosAtivos()
    {

        var response = await Client.GetAsync("/api/v1/produtos?apenasAtivos=true");
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        produtos.Should().NotBeNull();
        produtos.Should().AllSatisfy(p => p.Ativo.Should().BeTrue());
    }

    [Fact]
    public async Task ObterProduto_ComIdValido_DeveRetornarProduto()
    {

        var listResponse = await Client.GetAsync("/api/v1/produtos");
        var produtos = await listResponse.Content.ReadFromJsonAsync<List<ProdutoResponse>>();
        var primeiroId = produtos!.First().Id;


        var response = await Client.GetAsync($"/api/v1/produtos/{primeiroId}");


        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var produto = await response.Content.ReadFromJsonAsync<ProdutoResponse>();
        produto.Should().NotBeNull();
        produto!.Id.Should().Be(primeiroId);
    }

    [Fact]
    public async Task ObterProduto_ComIdInvalido_DeveRetornarNotFound()
    {

        var idInexistente = 99999;


        var response = await Client.GetAsync($"/api/v1/produtos/{idInexistente}");


        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListarProdutos_DeveRetornarProdutosOrdenadosPorNome()
    {

        var response = await Client.GetAsync("/api/v1/produtos");
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();


        produtos.Should().NotBeNull();
        produtos.Should().BeInAscendingOrder(p => p.Nome);
    }

    [Theory]
    [InlineData("CDB")]
    [InlineData("LCI")]
    [InlineData("LCA")]
    [InlineData("TesouroSelic")]
    public async Task ListarProdutos_ComDiferentesTipos_DeveRetornarProdutosCorretos(string tipo)
    {

        var response = await Client.GetAsync($"/api/v1/produtos?tipo={tipo}");
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        produtos.Should().NotBeNull();

        if (produtos!.Any())
        {
            produtos.Should().AllSatisfy(p => p.Tipo.Should().Be(tipo));
        }
    }

    [Theory]
    [InlineData("MuitoBaixo")]
    [InlineData("Baixo")]
    [InlineData("Medio")]
    [InlineData("Alto")]
    public async Task ListarProdutos_ComDiferentesRiscos_DeveRetornarProdutosCorretos(string risco)
    {

        var response = await Client.GetAsync($"/api/v1/produtos?risco={risco}");
        var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        produtos.Should().NotBeNull();

        if (produtos!.Any())
        {
            produtos.Should().AllSatisfy(p => p.Risco.Should().Be(risco));
        }
    }
}
