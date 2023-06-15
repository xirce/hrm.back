using IdentityServer4.Models;

namespace SkillSystem.IdentityServer4.Configuration;

public class IdentityServerSettings
{
    public string IssuerUri { get; set; }
    public string[] AllowedCorsOrigins { get; set; }
    public Client[] Clients { get; set; }
    public UsersApiSettings UsersApiSettings { get; set; }
}
