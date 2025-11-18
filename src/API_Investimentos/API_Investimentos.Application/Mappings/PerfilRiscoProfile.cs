using API_Investimentos.Application.DTOs.Responses;
using API_Investimentos.Domain.Entities;
using AutoMapper;

namespace API_Investimentos.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para PerfilRisco
/// </summary>
public class PerfilRiscoProfile : Profile
{
    public PerfilRiscoProfile()
    {
        CreateMap<PerfilRisco, PerfilRiscoResponse>()
            .ForMember(dest => dest.Perfil, opt => opt.MapFrom(src => src.Perfil.ToString()))
            .ForMember(dest => dest.UltimaAtualizacao, opt => opt.MapFrom(src => src.AtualizadoEm ?? src.CriadoEm))
            .ForMember(dest => dest.ProximaAvaliacao, opt => opt.MapFrom(src => src.DataProximaAvaliacao));
    }
}
