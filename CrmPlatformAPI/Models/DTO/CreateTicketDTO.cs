
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

        public string Language { get; set; }

        public string LanguageCode { get; set; }

        public string CountryCode { get; set; }

        public IFormFileCollection? Attachments { get; set; }  


    }
}
