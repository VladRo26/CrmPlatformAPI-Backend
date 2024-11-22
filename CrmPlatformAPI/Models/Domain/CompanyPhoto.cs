namespace CrmPlatformAPI.Models.Domain
{
    public class CompanyPhoto
    {
        public int Id { get; set; }
        public required string Url { get; set; }

        public string? PublicId { get; set; }

        public int? BeneficiaryCompanyId { get; set; }
        public BeneficiaryCompany? BeneficiaryCompany { get; set; }

        public int? SoftwareCompanyId { get; set; }
        public SoftwareCompany? SoftwareCompany { get; set; }
    }
}
