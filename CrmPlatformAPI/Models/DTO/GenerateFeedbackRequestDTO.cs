namespace CrmPlatformAPI.Models.DTO
{
    public class GenerateFeedbackRequestDTO
    {
        public string Username { get; set; }
        public int TicketId { get; set; }
        public int Rating { get; set; }
        public string UserExperience { get; set; }
    }
}
