namespace ElasticKafka.Client.Messaging;

internal static class GatewayUrls
{
    private const string Urls = "http://localhost:5100";

    public static string Get()
    {
        var urls = Urls.Split(',');
        
        if (urls.Length is 1)
            return urls[0];
        
        var idx = Random.Shared.Next(0, urls.Length);

        return urls[idx];
    }
}