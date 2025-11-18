using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Entities;
using AutoMapper;

namespace API_Investimentos.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para Produto
/// </summary>
public class ProdutoProfile : Profile
{
    public ProdutoProfile()
    {
        CreateMap<Produto, ProdutoResponse>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
            .ForMember(dest => dest.Risco, opt => opt.MapFrom(src => src.NivelRisco.ToString()))
            .ForMember(dest => dest.Rentabilidade, opt => opt.MapFrom(src => src.TaxaRentabilidade.Valor))
            .ForMember(dest => dest.ValorMinimo, opt => opt.MapFrom(src => src.ValorMinimo.Valor))
            .ForMember(dest => dest.TaxaAdministracao, opt => opt.MapFrom(src => src.TaxaAdministracao != null ? src.TaxaAdministracao.Valor : (decimal?)null))
            .ForMember(dest => dest.TaxaPerformance, opt => opt.MapFrom(src => src.TaxaPerformance != null ? src.TaxaPerformance.Valor : (decimal?)null));
    }
}
