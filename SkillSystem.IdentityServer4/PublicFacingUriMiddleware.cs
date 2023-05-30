using IdentityServer4.Extensions;

namespace SkillSystem.IdentityServer4;

public class PublicFacingUriMiddleware
{
    private readonly string publicFacingUri;
    private readonly RequestDelegate next;

    public PublicFacingUriMiddleware(string publicFacingUri, RequestDelegate next)
    {
        this.publicFacingUri = publicFacingUri;
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;

        context.SetIdentityServerOrigin(publicFacingUri);
        context.SetIdentityServerBasePath(request.PathBase.Value?.TrimEnd('/'));

        await next(context);
    }
}
