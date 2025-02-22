namespace CrmPlatformAPI.Models.DTO
{
    public class CreateBeneficiaryCompanyWithPhotoDTO
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? ActivityDomain { get; set; }
        public string? Address { get; set; }
        public DateOnly EstablishmentDate { get; set; }
        // File for company photo:
        public IFormFile? File { get; set; }
    }
}
