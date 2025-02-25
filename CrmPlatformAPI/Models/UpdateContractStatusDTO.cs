using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models
{
    public class UpdateContractStatusDTO
    {
        [Required]
        public float Status { get; set; }
    }
}
