namespace CrmPlatformAPI.Models.DTO
{
    public class TicketGroupedByCompanyDTO
    {

        public int BeneficiaryCompanyId { get; set; }
        public string BeneficiaryCompanyName { get; set; }
        public int TotalTickets { get; set; }
        public List<TicketStatusDTO> TicketsByStatus { get; set; } = new();
    }
}
