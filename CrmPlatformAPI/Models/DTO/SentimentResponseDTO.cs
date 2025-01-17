namespace CrmPlatformAPI.Models.DTO
{
    public class SentimentResponseDTO
    {
        public List<string> Labels { get; set; }
        public List<float> Scores { get; set; }
    }
}
