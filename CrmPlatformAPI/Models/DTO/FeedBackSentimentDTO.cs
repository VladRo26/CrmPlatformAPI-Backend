using CrmPlatformAPI.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmPlatformAPI.Models.DTO
{
    public class FeedBackSentimentDTO
    {
        public int FeedbackId { get; set; }
        public float Positive { get; set; }
        public float Neutral { get; set; }
        public float Negative { get; set; }
    }
}
