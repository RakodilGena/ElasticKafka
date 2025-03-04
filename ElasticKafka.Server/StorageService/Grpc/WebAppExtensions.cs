namespace StorageService.Grpc;

internal static class WebAppExtensions
{
    public static WebApplication MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<StoredMessagesService>();

        return app;
    }
}