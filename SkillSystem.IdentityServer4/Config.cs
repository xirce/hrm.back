using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace SkillSystem.IdentityServer4;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new("roles", "Roles", new[] { JwtClaimTypes.Role })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("SkillSystem.WebApi")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new("SkillSystem.WebApi", "Skill System Web Api", new[] { JwtClaimTypes.Role })
            {
                Scopes = { "SkillSystem.WebApi" }
            }
        };
}
