﻿namespace ElasticKafka.Client.Messaging;

internal static class GatewayUrls
{
    //local
    //private const string Urls = "http://localhost:5100";
    
    //docker cluster
    private const string Urls = "http://localhost:5101,http://localhost:5102";

    public static string Get()
    {
        var urls = Urls.Split(',');
        
        if (urls.Length is 1)
            return urls[0];
        
        var idx = Random.Shared.Next(0, urls.Length);

        return urls[idx];
    }
}