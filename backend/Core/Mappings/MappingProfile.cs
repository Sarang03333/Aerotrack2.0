using AutoMapper;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Core.DTOs;

namespace AeroTrack.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Aircraft: Map DTO to Entity and ensure ComplianceStatus isn't null
        CreateMap<AircraftCreateDto, Aircraft>()
            .ForMember(dest => dest.ComplianceStatus, opt => opt.MapFrom(src => "Pending"));

        // Maintenance: Map Task DTOs
        CreateMap<MaintenanceTaskCreateDto, MaintenanceTask>().ReverseMap();

        // Inventory: Map Spare Parts
        CreateMap<SparePartCreateDto, SparePart>().ReverseMap();

        // Compliance: Added mapping for Audit Logs (fixes missing service date issues)
        CreateMap<AuditCreateDto, AuditLog>().ReverseMap();
    }
}