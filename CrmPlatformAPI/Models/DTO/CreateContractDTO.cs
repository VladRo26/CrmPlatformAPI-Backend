namespace CrmPlatformAPI.Models.DTO
{
    public class CreateContractDTO
    {
        public string ProjectName { get; set; }
        public DateOnly StartDate { get; set; }
        public decimal Budget { get; set; }
        public DateOnly EstimatedFinishDate { get; set; }
        public bool OffersSupport { get; set; }

        public string? BeneficiaryCompanyName { get; set; } 
        public string? SoftwareCompanyName { get; set; }    
    }
}
