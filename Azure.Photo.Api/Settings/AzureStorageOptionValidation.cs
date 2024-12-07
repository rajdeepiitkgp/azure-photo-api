using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Settings;

public class AzureStorageOptionValidation : IValidateOptions<AzureStorageOption>
{
    public ValidateOptionsResult Validate(string? name, AzureStorageOption option)
    {
        if (string.IsNullOrEmpty(option.AccountName))
            return ValidateOptionsResult.Fail($"{nameof(option.AccountName)} cannot be null/empty");
        if (string.IsNullOrEmpty(option.ContainerName))
            return ValidateOptionsResult.Fail($"{nameof(option.ContainerName)} cannot be null/empty");

        return ValidateOptionsResult.Success;
    }
}
