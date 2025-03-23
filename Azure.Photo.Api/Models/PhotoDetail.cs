using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Azure.Photo.Api.Models;

public class PhotoDetail
{
    [JsonPropertyName("Id")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("Url")]
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}
