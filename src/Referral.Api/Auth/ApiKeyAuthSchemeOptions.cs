using Microsoft.AspNetCore.Authentication;

namespace Referral.Api.Auth;

public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// In production this would come from key manager/db/keyvault
    /// </summary>
    public string ApiKey { get; set; } = "VerySecret"; 
}
