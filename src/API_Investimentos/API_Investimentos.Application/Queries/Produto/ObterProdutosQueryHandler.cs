using API_Investimentos.Application.Common;
using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.Produto;

/// <summary>
/// Handler para obter produtos
/// </summary>
public class ObterProdutosQueryHandler : IRequestHandler<ObterProdutosQuery, Result<List<ProdutoResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterProdutosQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ProdutoResponse>>> Handle(
        ObterProdutosQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Produto> produtos;

        if (!string.IsNullOrEmpty(request.Tipo))
        {
            if (!Enum.TryParse<TipoProduto>(request.Tipo, true, out var tipoProduto))
            {
                return Result<List<ProdutoResponse>>.Falha($"Tipo de produto inválido: {request.Tipo}");
            }

            produtos = await _unitOfWork.Produtos.ObterPorTipoAsync(tipoProduto, cancellationToken);
        }
        else if (request.ApenasAtivos == true)
        {
            produtos = await _unitOfWork.Produtos.ObterAtivosAsync(cancellationToken);
        }
        else
        {
            produtos = await _unitOfWork.Produtos.ObterTodosAsync(cancellationToken);
        }

        var response = produtos.Select(p => new ProdutoResponse
        {
            Id = p.Id,
            Nome = p.Nome,
            Tipo = p.Tipo.ToString(),
            Rentabilidade = p.TaxaRentabilidade.Valor,
            Risco = p.NivelRisco.ToString(),
            ValorMinimo = p.ValorMinimo.Valor,
            PrazoMinimoMeses = p.PrazoMinimoMeses,
            PrazoMaximoMeses = p.PrazoMaximoMeses,
            LiquidezDiaria = p.LiquidezDiaria,
            Ativo = p.Ativo,
            IsentoIR = p.IsentoIR,
            TaxaAdministracao = p.TaxaAdministracao?.Valor,
            TaxaPerformance = p.TaxaPerformance?.Valor
        }).ToList();

        return Result<List<ProdutoResponse>>.Ok(response);
    }
}
