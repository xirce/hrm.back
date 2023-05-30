using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillSystem.IdentityServer4.Configuration;
using SkillSystem.IdentityServer4.Data;
using SkillSystem.IdentityServer4.Data.Entities;

namespace SkillSystem.IdentityServer4;

public class Startup
{
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;
    private readonly IdentityServerSettings settings;

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
        this.environment = environment;
        this.configuration = configuration;
        settings = this.configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddDbContext<SkillSystemIdentityDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("SkillSystemIdentity")));
        services.AddScoped<SkillSystemIdentityDbInitializer>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
            .AddEntityFrameworkStores<SkillSystemIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(settings.Clients)
            .AddDeveloperSigningCredential()
            .AddAspNetIdentity<ApplicationUser>();
    }

    public void Configure(IApplicationBuilder app)
    {
        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.EnsureUsersSeeded();
        }

        app.UseMiddleware<PublicFacingUriMiddleware>(settings.IssuerUri);

        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();
        app.UseCors(
            options => options.WithOrigins(settings.AllowedCorsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod());
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
    }
}
