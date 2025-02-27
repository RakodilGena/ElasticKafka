using Microsoft.Extensions.DependencyInjection;

namespace ElasticKafka.Client.Messaging;

internal static class GlobalServiceProvider
{
    private static IServiceProvider _provider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
        
    public static TService GetService<TService>() where TService : notnull
    {
        return _provider.GetRequiredService<TService>();
    }
    
    //public static IServiceScope CreateScope() => _provider.
}