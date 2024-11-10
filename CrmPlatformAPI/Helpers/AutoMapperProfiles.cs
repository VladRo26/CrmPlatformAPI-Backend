using AutoMapper;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;

namespace CrmPlatformAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // BeneficiaryCompany mappings
            CreateMap<BeneficiaryCompany, BeneficiaryCompanyDTO>();
            CreateMap<CreateBeneficiaryCompanyDTO, BeneficiaryCompany>();

            // SoftwareCompany mappings
            CreateMap<SoftwareCompany, SoftwareCompanyDTO>();
            CreateMap<CreateSoftwareCompanyDTO, SoftwareCompany>();

            // User mappings for registration
            CreateMap<User, RegisterDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom<CompanyNameResolver>())
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType));

            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore());

            // User mappings for UserDTO
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.UserType == UserType.SoftwareCompanyUser
                    ? src.SoftwareCompany.Name
                    : src.BeneficiaryCompany.Name))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType));

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore());

            // HomeImage mappings
            CreateMap<ImageDTO, HomeImage>();
            CreateMap<HomeImage, ImageDTO>();

            // Contract mappings
            CreateMap<Contract, ContractDTO>()
            .ForMember(dto => dto.BeneficiaryCompanyName, opt => opt.MapFrom(src => src.BeneficiaryCompany.Name))
            .ForMember(dto => dto.SoftwareCompanyName, opt => opt.MapFrom(src => src.SoftwareCompany.Name));

            CreateMap<ContractDTO, Contract>();

        }
    }
}
