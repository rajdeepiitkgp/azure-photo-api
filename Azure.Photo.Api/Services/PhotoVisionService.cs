using Azure.AI.Vision.ImageAnalysis;
using Azure.Identity;
using Azure.Photo.Api.Interfaces;
using Azure.Photo.Api.Models;
using Azure.Photo.Api.Settings;
using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Services;

public class PhotoVisionService(IOptions<UserIdentityConfig> userIdentityConfig, IOptions<ComputerVisionConfig> computerVisionConfig) : IPhotoVisionService
{
    private readonly UserIdentityConfig _userIdentityConfig = userIdentityConfig.Value;
    private readonly ComputerVisionConfig _computerVisionConfig = computerVisionConfig.Value;

    private ImageAnalysisClient GetComputerVisionClient()
    {
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = _userIdentityConfig.ClientId });
        var computerVisionClient = new ImageAnalysisClient(new Uri($"https://{_computerVisionConfig.AccountName}.cognitiveservices.azure.com/"), credential);
        return computerVisionClient;
    }

    public async Task<PhotoImageAnalysisResult> AnalyzeImage(Uri imageUri)
    {
        var visionClient = GetComputerVisionClient();
        var analizeResult = await visionClient.AnalyzeAsync(imageUri, VisualFeatures.Tags | VisualFeatures.Caption);
        var results = new PhotoImageAnalysisResult
        {
            Url = imageUri.ToString(),
            Caption = analizeResult.Value.Caption.Text,
            Confidence = analizeResult.Value.Caption.Confidence.ToString("F4"),
            Tags = analizeResult.Value.Tags.Values.Select(t => new PhotoImageAnalysisResult.Tag { Name = t.Name, Confidence = t.Confidence.ToString("F4") })
        };

        return results;
    }
}
