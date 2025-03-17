using System.Diagnostics;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog;

namespace GatewayService.Extensions;

internal static class StartupExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ElasticKafka API",
                Description = "ElasticKafka app gateway API"

                // TermsOfService = new Uri("https://example.com/terms"),
                //
                // Contact = new OpenApiContact
                // {
                //     Name = "Example Contact",
                //     Url = new Uri("https://example.com/contact")
                // },
                //
                // License = new OpenApiLicense
                // {
                //     Name = "Example License",
                //     Url = new Uri("https://example.com/license")
                // }
            });

            //allows jwt bearer auth
            // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     In = ParameterLocation.Header,
            //     Description = "Please insert JWT with Bearer into field",
            //     Name = "Authorization",
            //     Type = SecuritySchemeType.ApiKey
            // });
            // options.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             }
            //         },
            //         []
            //     }
            // });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });


        return services;
    }

    public static IServiceCollection AddCorsDefaultPolicy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowOrigins =
            configuration.GetSection("AllowOrigins").Get<string>() ??
            string.Empty;

        string[] allowedOrigins = !string.IsNullOrEmpty(allowOrigins)
            ? allowOrigins.Split(",")
            : [];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader();
                    if (allowedOrigins.Length > 0)
                        builder.WithOrigins(allowedOrigins);
                    else
                        builder.SetIsOriginAllowed(_ => true);
                    builder.AllowCredentials();
                });
            Trace.TraceInformation(allowOrigins);
        });

        return services;
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

    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, _, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });
        builder.Services.AddLogging();
        
        return builder;
    }

    public static void UseSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        SwaggerBuilderExtensions.UseSwagger(app);
        app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "swagger";
        });
    }
}