using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace StorageService.Elastic;

internal static class ServiceCollectionExtensions
{
    public static bool InMigratorMode(this IConfiguration configuration)
    {
        var needMigration = configuration.GetValue<string>("MIGRATE") is "true";
        return needMigration;
    }

    public static IServiceCollection AddElastic(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var uriString = configuration["Elastic:Nodes"];

        if (string.IsNullOrEmpty(uriString))
            throw new Exception("Elastic:Nodes not configured");

        services.AddSingleton<ElasticsearchClient>(_ =>
        {
            var uris = uriString.Split(',').Select(u => new Uri(u));

            var nodePool = new StaticNodePool(uris);

            var settings = new ElasticsearchClientSettings(nodePool)
                .DefaultIndex(ElasticIndices.Messages)
                .EnableDebugMode()
                .EnableHttpCompression();

            return new ElasticsearchClient(settings);
        });

        return services;
    }
}