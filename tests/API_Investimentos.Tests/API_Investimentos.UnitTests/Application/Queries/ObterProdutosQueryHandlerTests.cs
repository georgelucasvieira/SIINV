using System.Reflection;
using API_Investimentos.Application.Queries.Produto;
using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using API_Investimentos.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace API_Investimentos.UnitTests.Application.Queries;

public class ObterProdutosQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepositorioProduto _repositorioProduto;
    private readonly ObterProdutosQueryHandler _handler;

    public ObterProdutosQueryHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _repositorioProduto = Substitute.For<IRepositorioProduto>();
        _unitOfWork.Produtos.Returns(_repositorioProduto);
        _handler = new ObterProdutosQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_SemFiltro_DeveRetornarTodosProdutos()
    {
        var produtos = CriarListaProdutos();
        _repositorioProduto.ObterTodosAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Produto>>(produtos));

        var query = new ObterProdutosQuery();

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ComTipoValido_DeveRetornarProdutosFiltrados()
    {
        var produtos = new List<Produto> { CriarProduto(TipoProduto.CDB) };
        _repositorioProduto.ObterPorTipoAsync(TipoProduto.CDB, Arg.Any<CancellationToken>())
            .Returns(produtos);

        var query = new ObterProdutosQuery { Tipo = "CDB" };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().HaveCount(1);
        resultado.Dados![0].Tipo.Should().Be("CDB");
    }

    [Fact]
    public async Task Handle_ComTipoInvalido_DeveRetornarFalha()
    {
        var query = new ObterProdutosQuery { Tipo = "TipoInvalido" };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("Tipo de produto inválido");
    }

    [Fact]
    public async Task Handle_ApenasAtivos_DeveRetornarProdutosAtivos()
    {
        var produtos = CriarListaProdutos();
        _repositorioProduto.ObterAtivosAsync(Arg.Any<CancellationToken>())
            .Returns(produtos);

        var query = new ObterProdutosQuery { ApenasAtivos = true };

        var resultado = await _handler.Handle(query, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        await _repositorioProduto.Received(1).ObterAtivosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveMapearProdutoCorretamente()
    {
        var produto = CriarProduto(TipoProduto.LCI);
        _repositorioProduto.ObterTodosAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Produto> { produto });

        var query = new ObterProdutosQuery();

        var resultado = await _handler.Handle(query, CancellationToken.None);

        var response = resultado.Dados![0];
        response.Nome.Should().Be(produto.Nome);
        response.Tipo.Should().Be("LCI");
        response.Risco.Should().Be("Medio");
        response.IsentoIR.Should().BeTrue();
    }

    private List<Produto> CriarListaProdutos()
    {
        return new List<Produto>
        {
            CriarProduto(TipoProduto.CDB),
            CriarProduto(TipoProduto.LCI)
        };
    }

    private Produto CriarProduto(TipoProduto tipo)
    {
        var produto = new Produto(
            $"Produto {tipo}",
            tipo,
            NivelRisco.Medio,
            Percentual.CriarDePercentual(10m),
            Dinheiro.Criar(100),
            1,
            false,
            tipo == TipoProduto.LCI || tipo == TipoProduto.LCA
        );

        var idProperty = typeof(Produto).GetProperty("Id");
        idProperty?.SetValue(produto, (long)tipo + 1);

        return produto;
    }
}
