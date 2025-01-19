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
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo.Url))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType.ToString())); // Convert enum to string



            CreateMap<UserAppDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => Enum.Parse<UserType>(src.UserType))); // Convert string back to enum



            CreateMap<User, UpdateUserDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo.Url));
            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore());



            // HomeImage mappings
            CreateMap<ImageDTO, HomeImage>();
            CreateMap<HomeImage, ImageDTO>();

            CreateMap<ImageDTO, Photo>();
            CreateMap<Photo, ImageDTO>();

            // Contract mappings
            CreateMap<Contract, ContractDTO>()
            .ForMember(dto => dto.BeneficiaryCompanyName, opt => opt.MapFrom(src => src.BeneficiaryCompany.Name))
            .ForMember(dto => dto.SoftwareCompanyName, opt => opt.MapFrom(src => src.SoftwareCompany.Name))
            .ForMember(dto => dto.BeneficiaryCompanyPhotoUrl, opt => opt.MapFrom(c => c.BeneficiaryCompany.CompanyPhoto.Url))
            .ForMember(dto => dto.SoftwareCompanyPhotoUrl, opt => opt.MapFrom(c => c.SoftwareCompany.CompanyPhoto.Url));

            CreateMap<CreateContractDTO, Contract>()
                .ForMember(dest => dest.BeneficiaryCompany, opt => opt.Ignore())
                .ForMember(dest => dest.SoftwareCompany, opt => opt.Ignore());

            CreateMap<Ticket, TicketDTO>()
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString())); // Convert enum to string


            CreateMap<TicketStatusHistory, TicketStatusHistoryDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.UpdatedByUsername, opt => opt.MapFrom(src => src.UpdatedByUser.UserName)) // Map username
                .ForMember(dest => dest.TicketUserRole, opt => opt.MapFrom(src => src.TicketUserRole.ToString()));

         CreateMap<TicketStatusHistoryDTO, TicketStatusHistory>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore()) // Handle user separately
                .ForMember(dest => dest.TicketUserRole, opt => opt.MapFrom(src => Enum.Parse<TicketUserRole>(src.TicketUserRole)));


            CreateMap<Feedback, FeedbackDTO>()
                .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dto => dto.FromUserId, opt => opt.MapFrom(src => src.FromUserId))
                .ForMember(dto => dto.ToUserId, opt => opt.MapFrom(src => src.ToUserId))
                .ForMember(dto => dto.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dto => dto.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dto => dto.Rating, opt => opt.MapFrom(src => src.Rating));


            CreateMap<FeedbackDTO, Feedback>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.FromUserId, opt => opt.MapFrom(src => src.FromUserId))
                .ForMember(dest => dest.ToUserId, opt => opt.MapFrom(src => src.ToUserId))
                .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.FromUser, opt => opt.Ignore()) // Ignore navigation properties for manual handling
                .ForMember(dest => dest.ToUser, opt => opt.Ignore())
                .ForMember(dest => dest.Ticket, opt => opt.Ignore());


            CreateMap<FeedBackSentiment, FeedBackSentimentDTO>()
               .ReverseMap(); // Enables reverse mapping if needed

        }
    }
}
