namespace CleanArchitecture.WebApi.Middleware;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseMiddleWareExtensions(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}
