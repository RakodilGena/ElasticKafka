using ServiceDiscovery.Services;

namespace ServiceDiscovery.Extensions;

internal static class StartupExtensions
{
    public static WebApplication MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<ServiceUrlsStreamer>();

        return app;
    }
}