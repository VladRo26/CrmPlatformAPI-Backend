namespace CrmPlatformAPI.Models.DTO
{
    public class CreateBeneficiaryCompanyDTO
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? ActivityDomain { get; set; }
        public string? Address { get; set; }
        public DateOnly EstablishmentDate { get; set; }
        public string PhotoUrl { get; set; }

    }
}
