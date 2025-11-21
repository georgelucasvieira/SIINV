using API_Investimentos.Application.Common;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.Interfaces;
using MediatR;

namespace API_Investimentos.Application.Queries.Produto;

/// <summary>
/// Handler para obter produtos recomendados por perfil
/// </summary>
public class ObterProdutosRecomendadosQueryHandler
    : IRequestHandler<ObterProdutosRecomendadosQuery, Result<List<ProdutoRecomendadoResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterProdutosRecomendadosQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ProdutoRecomendadoResponse>>> Handle(
        ObterProdutosRecomendadosQuery request,
        CancellationToken cancellationToken)
    {

        if (!Enum.TryParse<PerfilInvestidor>(request.Perfil, true, out var perfil))
        {
            return Result<List<ProdutoRecomendadoResponse>>.Falha(
                $"Perfil inválido. Valores aceitos: {string.Join(", ", Enum.GetNames<PerfilInvestidor>())}");
        }


        var produtos = await _unitOfWork.Produtos.ObterTodosAsync(cancellationToken);
        var produtosAtivos = produtos.Where(p => p.Ativo).ToList();


        var niveisRiscoPermitidos = ObterNiveisRiscoPorPerfil(perfil);

        var produtosRecomendados = produtosAtivos
            .Where(p => niveisRiscoPermitidos.Contains(p.NivelRisco))
            .OrderByDescending(p => p.TaxaRentabilidade.Valor)
            .Select(p => new ProdutoRecomendadoResponse
            {
                Id = p.Id,
                Nome = p.Nome,
                Tipo = p.Tipo.ToString(),
                Rentabilidade = p.TaxaRentabilidade.Valor,
                Risco = p.NivelRisco.ToString()
            })
            .ToList();

        return Result<List<ProdutoRecomendadoResponse>>.Ok(produtosRecomendados);
    }

    private static List<NivelRisco> ObterNiveisRiscoPorPerfil(PerfilInvestidor perfil)
    {
        return perfil switch
        {
            PerfilInvestidor.Conservador => new List<NivelRisco>
            {
                NivelRisco.MuitoBaixo,
                NivelRisco.Baixo
            },
            PerfilInvestidor.Moderado => new List<NivelRisco>
            {
                NivelRisco.MuitoBaixo,
                NivelRisco.Baixo,
                NivelRisco.Medio
            },
            PerfilInvestidor.Agressivo => new List<NivelRisco>
            {
                NivelRisco.MuitoBaixo,
                NivelRisco.Baixo,
                NivelRisco.Medio,
                NivelRisco.Alto,
                NivelRisco.MuitoAlto
            },
            _ => new List<NivelRisco>()
        };
    }
}
