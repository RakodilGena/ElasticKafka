using Serilog;

namespace StorageService.Extensions;

internal static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, _, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });
        builder.Services.AddLogging();
        
        return builder;
    }
    
    public static WebApplicationBuilder ValidateServicesLifetimes(
        this WebApplicationBuilder builder)
    {
        builder.WebHost.UseDefaultServiceProvider(
            (_, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            });

        return builder;
    }
}