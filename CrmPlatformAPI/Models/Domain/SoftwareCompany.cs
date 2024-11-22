namespace CrmPlatformAPI.Models.Domain
{
    public class SoftwareCompany
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public DateOnly EstablishmentDate { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        public CompanyPhoto? CompanyPhoto { get; set; }


    }
}
