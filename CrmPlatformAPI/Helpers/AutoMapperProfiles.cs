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
            CreateMap<BeneficiaryCompany, BeneficiaryCompanyDTO>()
            .ForMember(dto => dto.PhotoUrl, opt => opt.MapFrom(src => src.CompanyPhoto.Url));
            CreateMap<CreateBeneficiaryCompanyDTO, BeneficiaryCompany>()
            .ForMember(dest => dest.CompanyPhoto, opt => opt.Ignore());


            // SoftwareCompany mappings
            CreateMap<SoftwareCompany, SoftwareCompanyDTO>()
                .ForMember(dto => dto.PhotoUrl, opt => opt.MapFrom(src => src.CompanyPhoto.Url));
            CreateMap<CreateSoftwareCompanyDTO, SoftwareCompany>()
                .ForMember(dest => dest.CompanyPhoto, opt => opt.Ignore());

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
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo.Url));

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore());
            // user mapping for UserAppDTO
            CreateMap<User, UserAppDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.UserType == UserType.SoftwareCompanyUser
                                   ? src.SoftwareCompany.Name
                                                      : src.BeneficiaryCompany.Name))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo.Url));

            CreateMap<UserAppDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore());


            CreateMap<User, UpdateUserDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo.Url));
            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore());

            // HomeImage mappings
            CreateMap<ImageDTO, HomeImage>();
            CreateMap<HomeImage, ImageDTO>();

            // Contract mappings
            CreateMap<Contract, ContractDTO>()
            .ForMember(dto => dto.BeneficiaryCompanyName, opt => opt.MapFrom(src => src.BeneficiaryCompany.Name))
            .ForMember(dto => dto.SoftwareCompanyName, opt => opt.MapFrom(src => src.SoftwareCompany.Name))
            .ForMember(dto => dto.BeneficiaryCompanyPhotoUrl, opt => opt.MapFrom(c => c.BeneficiaryCompany.CompanyPhoto.Url))
            .ForMember(dto => dto.SoftwareCompanyPhotoUrl, opt => opt.MapFrom(c => c.SoftwareCompany.CompanyPhoto.Url));

            CreateMap<CreateContractDTO, Contract>()
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore())
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore());




        }
    }
}
