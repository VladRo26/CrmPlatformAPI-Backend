namespace CrmPlatformAPI.Models.DTO
{
    public class SoftwareCompanyDTO
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public DateOnly EstablishmentDate { get; set; }

        public string PhotoUrl { get; set; }

    }
}
