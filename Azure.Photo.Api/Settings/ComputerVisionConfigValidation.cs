using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Settings;

public class ComputerVisionConfigValidation : IValidateOptions<ComputerVisionConfig>
{
    public ValidateOptionsResult Validate(string? name, ComputerVisionConfig option)
    {
        if (string.IsNullOrEmpty(option.AccountName))
            return ValidateOptionsResult.Fail($"{nameof(option.AccountName)} cannot be null/empty");

        return ValidateOptionsResult.Success;
    }
}