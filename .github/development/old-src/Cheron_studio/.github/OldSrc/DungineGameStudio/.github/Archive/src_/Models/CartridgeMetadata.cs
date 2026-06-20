using System.Text.Json.Serialization;

namespace DungineStudio.Models
{
    public class CartridgeMetadata
    {
        [JsonPropertyName("boxArt")]
        public string? BoxArt { get; set; }

        [JsonPropertyName("labelArt")]
        public string? LabelArt { get; set; }
    }
}
