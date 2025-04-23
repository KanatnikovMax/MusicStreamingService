using Duende.IdentityModel;
using Duende.IdentityServer.Models;

namespace MusicStreamingService.Service.Settings;

public static class IdentityServerConfigSettings
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource(
            name: "roles",
            displayName: "Roles",
            userClaims: [JwtClaimTypes.Role])
    ];
    
    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("api", "Music API") 
        {
            Scopes = ["api"] 
        }
    ];
    
    public static IEnumerable<ApiScope> ApiScopes => [new ApiScope("api", "Music API")];

    public static IEnumerable<Client> GetClients(MusicServiceSettings settings) =>
    [
        new Client
        {
            ClientName = settings.ClientId,
            ClientId = settings.ClientId,
            Enabled = true,
            AllowOfflineAccess = true,
            AllowedGrantTypes =
            [
                GrantType.ClientCredentials,
                GrantType.ResourceOwnerPassword
            ],
            ClientSecrets = [new Secret(settings.ClientSecret.Sha256())],
            AllowedScopes = ["api", "openid", "profile", "roles"]
        },
        new Client
        {
            ClientId = "swagger",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = [new Secret("swagger".Sha256())],
            AllowedScopes = ["api", "openid", "profile", "roles"]
            
        }
    ];
}