namespace BestPractices.AutomapperProfiles;
public class AutomapperProfile : Profile
{
  public AutomapperProfile()
  {
    CreateMap<Customer, CustomerDTO>()
    .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
    .ReverseMap();
    // CreateMap<CustomerDTO, Customer>();
  }
}
