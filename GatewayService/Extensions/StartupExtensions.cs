using System.Reflection;
using Microsoft.OpenApi.Models;

namespace GatewayService.Extensions;

internal static class StartupExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ElasticKafka API",
                Description = "ElasticKafka app gateway API",

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