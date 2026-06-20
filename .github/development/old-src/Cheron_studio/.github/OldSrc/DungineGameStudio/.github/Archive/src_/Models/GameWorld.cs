using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DungineStudio.Models
{
    public class GameWorld
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("startLocationId")]
        public string StartLocationId { get; set; } = string.Empty;

        [JsonPropertyName("genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("locations")]
        public List<Location> Locations { get; set; } = new();
    }

    public class Location
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("exits")]
        public Dictionary<string, string> Exits { get; set; } = new();

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("aliases")]
        public List<string>? Aliases { get; set; }

        [JsonPropertyName("isPortable")]
        public bool IsPortable { get; set; } = true;
    }
}
