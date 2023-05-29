using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SkillSystem.WebApi.Configuration;

public class SwaggerGenSetup : IConfigureOptions<SwaggerGenOptions>
{
    private readonly SkillSystemWebApiSettings settings;

    public SwaggerGenSetup(SkillSystemWebApiSettings settings)
    {
        this.settings = settings;
    }

    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(
            "oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(settings.IdentityBaseUrl + "/connect/authorize"),
                        TokenUrl = new Uri(settings.IdentityBaseUrl + "/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            ["SkillSystem.WebApi"] = "Skill System Web Api"
                        }
                    }
                }
            });

        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

        var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
}
