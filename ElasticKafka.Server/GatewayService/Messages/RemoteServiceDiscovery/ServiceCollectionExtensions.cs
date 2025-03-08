using GatewayService.Messages.RemoteServiceDiscovery.Consumers;
using GatewayService.Messages.RemoteServiceDiscovery.Consumers.Impl;
using GatewayService.Messages.RemoteServiceDiscovery.Messages;
using GatewayService.Messages.RemoteServiceDiscovery.Messages.Impl;
using GatewayService.Messages.RemoteServiceDiscovery.Storage;
using GatewayService.Messages.RemoteServiceDiscovery.Storage.Impl;
using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using ServiceDiscovery;

namespace GatewayService.Messages.RemoteServiceDiscovery;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceDiscovery(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddOptions<MessageServicesUrls>()
        //     .BindConfiguration(MessageServicesUrls.SectionName)
        //     .ValidateDataAnnotations()
        //     .ValidateOnStart();
        //
        // services.AddOptions<StorageServicesUrls>()
        //     .BindConfiguration(StorageServicesUrls.SectionName)
        //     .ValidateDataAnnotations()
        //     .ValidateOnStart();

        services.AddBalancedSdClient(configuration);

        services.AddHostedService<ServiceDiscoveryBackgroundService>();

        services
            .AddSingleton<IServiceDiscoveryClient, ServiceDiscoveryClient>()
            .AddSingleton<MessagingServiceUrlProvider>()
            .AddSingleton<IMessagingServiceUrlProvider>(
                provider => provider.GetRequiredService<MessagingServiceUrlProvider>())
            .AddSingleton<IMessagingServiceUrlCollection>(
                provider => provider.GetRequiredService<MessagingServiceUrlProvider>())

            .AddSingleton<StorageServiceUrlProvider>()
            .AddSingleton<IStorageServiceUrlProvider>(
                provider => provider.GetRequiredService<StorageServiceUrlProvider>())
            .AddSingleton<IStorageServiceUrlCollection>(
                provider => provider.GetRequiredService<StorageServiceUrlProvider>());

        return services;
    }

    private static void AddBalancedSdClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceDiscoveryUrls = new ServiceDiscoveryUrls();

        configuration.GetSection(ServiceDiscoveryUrls.SectionName)
            .Bind(serviceDiscoveryUrls);

        if (serviceDiscoveryUrls.Value is null)
            throw new Exception($"{ServiceDiscoveryUrls.SectionName} are not configured");

        var addresses = serviceDiscoveryUrls.Value.Split(",")
            .Select(address => address.Split(':'))
            .Select(address => new BalancerAddress(
                host: address[0],
                port: int.Parse(address[1])))
            .ToArray();

        Console.WriteLine(
            $"Got service discovery addresses: {
                string.Join(", ", addresses.Select(a => a.ToString()))
            }");

        // var resolverFactory = new StaticResolverFactory(_ => addresses);
        //
        // services.AddSingleton<ResolverFactory>(resolverFactory);

        services.AddGrpcClient<ServiceDiscoveryRpc.ServiceDiscoveryRpcClient>(o =>
            {
                // Define static resolver factory with multiple addresses
                o.Address = new Uri("static:///");

                o.ChannelOptionsActions.Add(options =>
                {
                    options.ServiceConfig = new ServiceConfig
                    {
                        LoadBalancingConfigs = { new RoundRobinConfig() },
                        MethodConfigs =
                        {
                            new MethodConfig
                            {
                                //for all methods
                                Names = { MethodName.Default },
                                RetryPolicy = new RetryPolicy
                                {
                                    // Retry up to 5 times
                                    MaxAttempts = 5,
                                    InitialBackoff = TimeSpan.FromSeconds(1),
                                    MaxBackoff = TimeSpan.FromSeconds(5),

                                    //1 sec -> 2 -> 4 -> 5 -> 5 (cz of BackOff)
                                    BackoffMultiplier = 2,

                                    //only on Unavailable result
                                    RetryableStatusCodes = { StatusCode.Unavailable }
                                }
                            }
                        }
                    };

                    options.Credentials = ChannelCredentials.Insecure;
                });
            })
            .ConfigureChannel(options =>
            {

                var channelServices = new ServiceCollection();

                var factory = new StaticResolverFactory(_ => addresses);

                channelServices.AddSingleton<ResolverFactory>(factory);

                options.ServiceProvider = channelServices.BuildServiceProvider();
                //options.Credentials = ChannelCredentials.SecureSsl;
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                // use socket handler to support load balancing

                var socketsHandler = new SocketsHttpHandler();
                // Return `true` to allow certificates that are untrusted/invalid
                // socketsHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions {
                //     RemoteCertificateValidationCallback = delegate { return true; }
                // };

                return socketsHandler;
            });
    }
}