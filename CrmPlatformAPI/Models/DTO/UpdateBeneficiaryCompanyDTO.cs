using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class UpdateBeneficiaryCompanyDTO
    {
        [Required]
        public string Name { get; set; }

        public string? ShortDescription { get; set; }

        public string? ActivityDomain { get; set; }

        public string? Address { get; set; }

        [Required]
        public DateOnly EstablishmentDate { get; set; }

        // Optional file for updating the company photo
        public IFormFile? File { get; set; }
    }
}
