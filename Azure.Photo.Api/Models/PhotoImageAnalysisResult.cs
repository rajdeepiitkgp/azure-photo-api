using Newtonsoft.Json;

namespace Azure.Photo.Api.Models;

public class PhotoImageAnalysisResult
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("caption")]
    public string Caption { get; set; } = string.Empty;

    [JsonProperty("confidence")]
    public string Confidence { get; set; } = string.Empty;

    [JsonProperty("tags")]
    public IEnumerable<Tag> Tags { get; set; } = [];

    public class Tag
    {
        [JsonProperty("name")]

        public string Name { get; set; } = string.Empty;
        [JsonProperty("confidence")]

        public string Confidence { get; set; } = string.Empty;
    }
}
