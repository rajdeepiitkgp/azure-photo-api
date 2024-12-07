namespace Azure.Photo.Api.Models;

public class PhotoImageAnalysisResult
{
    public string Caption { get; set; } = string.Empty;
    public string Confidence { get; set; } = string.Empty;
    public IEnumerable<Tag> Tags { get; set; } = [];
    public class Tag
    {
        public string Name { get; set; } = string.Empty;
        public string Confidence { get; set; } = string.Empty;
    }
}
