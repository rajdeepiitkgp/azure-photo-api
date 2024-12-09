using Newtonsoft.Json;

namespace Azure.Photo.Api.Models;

public class PhotoMetadataResponse
{
    [JsonProperty("PhotoId")]
    public string Id { get; set; } = string.Empty;
    [JsonProperty("PhotoUrl")]

    public string Url { get; set; } = string.Empty;
    [JsonProperty("Tags")]

    public IEnumerable<string> Tags { get; set; } = [];
}