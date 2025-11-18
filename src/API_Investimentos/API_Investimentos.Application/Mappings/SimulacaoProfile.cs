using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Entities;
using AutoMapper;

namespace API_Investimentos.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para Simulacao
/// </summary>
public class SimulacaoProfile : Profile
{
    public SimulacaoProfile()
    {
        CreateMap<Simulacao, SimulacaoResponse>()
            .ForMember(dest => dest.ProdutoValidado, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Produto, opt => opt.MapFrom(src => src.Produto));

        CreateMap<Simulacao, ResultadoSimulacaoResponse>()
            .ForMember(dest => dest.ValorInvestido, opt => opt.MapFrom(src => src.ValorInvestido.Valor))
            .ForMember(dest => dest.ValorFinalBruto, opt => opt.MapFrom(src => src.ValorFinalBruto.Valor))
            .ForMember(dest => dest.ValorFinalLiquido, opt => opt.MapFrom(src => src.ValorFinalLiquido.Valor))
            .ForMember(dest => dest.RendimentoBruto, opt => opt.MapFrom(src => src.CalcularRendimentoBruto().Valor))
            .ForMember(dest => dest.RendimentoLiquido, opt => opt.MapFrom(src => src.CalcularRendimentoLiquido().Valor))
            .ForMember(dest => dest.ValorIR, opt => opt.MapFrom(src => src.ValorIR.Valor))
            .ForMember(dest => dest.AliquotaIR, opt => opt.MapFrom(src => src.AliquotaIR.Valor))
            .ForMember(dest => dest.RentabilidadeEfetiva, opt => opt.MapFrom(src => src.TaxaRentabilidadeEfetiva.Valor))
            .ForMember(dest => dest.RentabilidadeLiquida, opt => opt.MapFrom(src => src.CalcularRentabilidadeLiquida().Valor))
            .ForMember(dest => dest.DataSimulacao, opt => opt.MapFrom(src => src.CriadoEm));
    }
}
