using IdentityServer4.Models;

namespace SkillSystem.IdentityServer4.Configuration;

public class IdentityServerSettings
{
    public string[] AllowedCorsOrigins { get; set; }
    public Client[] Clients { get; set; }
}
