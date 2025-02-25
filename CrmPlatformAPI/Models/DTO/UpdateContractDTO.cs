using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class UpdateContractDTO
    {
        [Required]
        public string ProjectName { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [Required]
        public DateOnly EstimatedFinishDate { get; set; }

        [Required]
        public bool OffersSupport { get; set; }

        [Required]
        public float Status { get; set; }

        public string? Description { get; set; }
    }
}
