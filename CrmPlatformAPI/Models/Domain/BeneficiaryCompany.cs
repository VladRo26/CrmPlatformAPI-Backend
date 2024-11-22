namespace CrmPlatformAPI.Models.Domain
{
    public class BeneficiaryCompany
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? ActivityDomain { get; set; }
        public string? Address { get; set; }
        public DateOnly EstablishmentDate { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        public CompanyPhoto? CompanyPhoto { get; set; }



    }
}

