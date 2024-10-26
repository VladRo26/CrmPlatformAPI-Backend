using AutoMapper;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;

namespace CrmPlatformAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
    
        public AutoMapperProfiles()
        {
            CreateMap<BeneficiaryCompany, BeneficiaryCompanyDTO>();
            CreateMap<CreateBeneficiaryCompanyDTO, BeneficiaryCompany>();
            CreateMap<SoftwareCompany, SoftwareCompanyDTO>();
            CreateMap<CreateSoftwareCompanyDTO, SoftwareCompany>();
            CreateMap<User, RegisterDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom<CompanyNameResolver>());
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore());
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.SoftwareCompanyName, opt => opt.MapFrom(src => src.SoftwareCompany.Name));
            CreateMap<UserDTO, User>()
                    .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore());
            CreateMap<ImageDTO, HomeImage>();
            CreateMap<HomeImage, ImageDTO>();
        }
    }
}
