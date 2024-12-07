using System.Security.Policy;
using Newtonsoft.Json;

namespace Azure.Photo.Api.Models;

public class PhotoImageAnalysisResult
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Url { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Confidence { get; set; } = string.Empty;
    public IEnumerable<Tag> Tags { get; set; } = [];
    public class Tag
    {
        public string Name { get; set; } = string.Empty;
        public string Confidence { get; set; } = string.Empty;
    }
}
