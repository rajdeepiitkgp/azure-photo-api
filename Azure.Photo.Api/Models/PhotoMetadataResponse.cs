using System.Text.Json.Serialization;

namespace Azure.Photo.Api.Models;

public class PhotoMetadataResponse
{
    [JsonPropertyName("PhotoId")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("PhotoUrl")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("Caption")]
    public string Caption { get; set; } = string.Empty;

    [JsonPropertyName("Tags")]
    public IEnumerable<string> Tags { get; set; } = [];
}