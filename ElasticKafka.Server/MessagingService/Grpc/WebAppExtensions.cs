namespace MessagingService.Grpc;

internal static class WebAppExtensions
{
    public static WebApplication MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<MessagingService>();

        return app;
    }
}