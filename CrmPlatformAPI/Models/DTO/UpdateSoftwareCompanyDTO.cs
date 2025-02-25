using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class UpdateSoftwareCompanyDTO
    {

        [Required]
        public string Name { get; set; }

        public string? ShortDescription { get; set; }

        [Required]
        public DateOnly EstablishmentDate { get; set; }

        // Optional file for updating the company photo.
        public IFormFile? File { get; set; }
    }
}
