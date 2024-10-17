namespace CrmPlatformAPI.Models.DTO
{
    public class CreateSoftwareCompanyDTO
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public DateOnly EstablishmentDate { get; set; }
    }
}
