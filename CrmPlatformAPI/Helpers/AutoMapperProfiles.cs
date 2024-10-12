using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;

namespace CrmPlatformAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<BeneficiaryCompanies, BeneficiaryCompaniesDTO>();
            CreateMap<CreateBeneficiaryCompanyDTO, BeneficiaryCompanies>();
        }
    }
}
