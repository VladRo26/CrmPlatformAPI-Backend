using System.Text.Json.Serialization;

namespace CrmPlatformAPI.Models.DTO
{
    public class SentimentRequestDTO
    {
        [JsonPropertyName("text_content")]
        public string TextContent { get; set; }

    }
}
