namespace Azure.Photo.Api.Settings;

public class UserIdentityConfig
{
    public const string UserIdentity = nameof(UserIdentity);
    public string ClientId { get; set; } = string.Empty;
}
