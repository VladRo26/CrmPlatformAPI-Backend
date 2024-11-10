namespace CrmPlatformAPI.Models.DTO
{
    public class ContractDTO
    {
        public string ProjectName { get; set; }
        public DateOnly StartDate { get; set; }
        public decimal Budget { get; set; }
        public DateOnly EstimatedFinishDate { get; set; }
        public bool OffersSupport { get; set; }

        // Optional: Add navigation properties as simplified versions for DTO if needed
        public string BeneficiaryCompanyName { get; set; } // To display the name of the Beneficiary Company
        public string SoftwareCompanyName { get; set; }    // To display the name of the Software Company
    }
}
