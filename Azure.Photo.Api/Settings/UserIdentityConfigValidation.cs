using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Settings;

public class UserIdentityConfigValidation : IValidateOptions<UserIdentityConfig>
{
    public ValidateOptionsResult Validate(string? name, UserIdentityConfig option)
    {
        if (string.IsNullOrEmpty(option.ClientId))
            return ValidateOptionsResult.Fail($"{nameof(option.ClientId)} cannot be null/empty");

        return ValidateOptionsResult.Success;
    }
}
