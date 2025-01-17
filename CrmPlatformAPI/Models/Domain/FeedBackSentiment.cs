using System.ComponentModel.DataAnnotations.Schema;

namespace CrmPlatformAPI.Models.Domain
{
    public class FeedBackSentiment
    {
        public int Id { get; set; }

        [ForeignKey("Feedback")]
        public int FeedbackId { get; set; }
        public Feedback Feedback { get; set; }

        public float Positive { get; set; }
        public float Neutral { get; set; }
        public float Negative { get; set; }
    }
}
