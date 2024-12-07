using Azure.Photo.Api.Models;

namespace Azure.Photo.Api.Interfaces;

public interface IPhotoVisionService
{
    Task<PhotoImageAnalysisResult> AnalyzeImage(Uri imageUri);
}