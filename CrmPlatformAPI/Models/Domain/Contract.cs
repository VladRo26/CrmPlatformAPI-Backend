using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.Domain
{
    public class Contract
    {
        public int Id { get; set; }

        [ForeignKey("BeneficiaryCompany")]
        public int BeneficiaryCompanyId { get; set; }
        public BeneficiaryCompany BeneficiaryCompany { get; set; }

        [ForeignKey("SoftwareCompany")]
        public int SoftwareCompanyId { get; set; }
        public SoftwareCompany SoftwareCompany { get; set; }

        public string ProjectName { get; set; }
        public DateOnly StartDate { get; set; }
        public decimal Budget { get; set; }
        public DateOnly EstimatedFinishDate { get; set; }
        public bool OffersSupport { get; set; }

        public float Status { get; set; }
        
        public string? Description { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
