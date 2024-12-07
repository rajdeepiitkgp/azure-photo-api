using Microsoft.Extensions.Options;

namespace Azure.Photo.Api.Settings;

public class AzureCosmosDbConfigValidation : IValidateOptions<AzureCosmosDbConfig>
{
    public ValidateOptionsResult Validate(string? name, AzureCosmosDbConfig options)
    {
        if (string.IsNullOrEmpty(options.AccountName))
            return ValidateOptionsResult.Fail($"{nameof(options.AccountName)} cannot be null/empty");
        if (string.IsNullOrEmpty(options.ContainerName))
            return ValidateOptionsResult.Fail($"{nameof(options.ContainerName)} cannot be null/empty");
        if (string.IsNullOrEmpty(options.DbName))
            return ValidateOptionsResult.Fail($"{nameof(options.DbName)} cannot be null/empty");

        return ValidateOptionsResult.Success;
    }
}