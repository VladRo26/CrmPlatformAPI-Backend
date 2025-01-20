
namespace CrmPlatformAPI.Models.DTO
{
    public class CreateTicketDTO
    {

        public string? Title { get; set; }

        public string? Description { get; set; }


        public string Status { get; set; }

        public string? Priority { get; set; }

        public string? Type { get; set; }

        public int ContractId { get; set; }

        public int CreatorId { get; set; }

    }
}
