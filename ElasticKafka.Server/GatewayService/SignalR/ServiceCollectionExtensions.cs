namespace GatewayService.SignalR;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomSignalR(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnection = configuration["Redis:ConnectionString"];
        if (string.IsNullOrWhiteSpace(redisConnection))
        {
            throw new Exception("Redis connection string is missing.");
        }


        services.AddSignalR().AddStackExchangeRedis(redisConnection);

        services.AddScoped<IMessageCreatedEventsHubWrapper, MessageCreatedEventsHubWrapper>();

        return services;
    }

    public static WebApplication UseCustomSignalR(this WebApplication app)
    {
        app.MapHub<MessagesHub>("/messagesHub");

        return app;
    }
}