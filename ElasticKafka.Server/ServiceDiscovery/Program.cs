using Microsoft.AspNetCore.Server.Kestrel.Core;
using ServiceDiscovery.Extensions;
using ServiceDiscovery.Services;

var builder = WebApplication.CreateBuilder(args);

//validate whether services' lifetimes are valid and correspond to each other.
builder.WebHost.UseDefaultServiceProvider(
    (_, options) =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });

//for grpc only
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(x =>
        x.Protocols = HttpProtocols.Http2);
});

// Add services to the container.

builder.Services.AddGrpc();
builder.Services.AddServiceDiscovery();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcServices();


app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();