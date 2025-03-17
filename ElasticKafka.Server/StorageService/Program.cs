using Microsoft.AspNetCore.Server.Kestrel.Core;
using StorageService.Elastic;
using StorageService.Extensions;
using StorageService.Grpc;
using StorageService.Kafka;
using StorageService.Messages.Services;
using StorageService.Migration;

var builder = WebApplication.CreateBuilder(args);

var inMigratorMode = builder.Configuration.InMigratorMode();

//validate whether services' lifetimes are valid and correspond to each other.
builder.ValidateServicesLifetimes();

//for grpc only
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(x =>
        x.Protocols = HttpProtocols.Http2);
});

builder.ConfigureSerilog();

builder.Services
    .AddKafka(inMigratorMode)
    .AddElastic(builder.Configuration)
    .AddMessageServices();

builder.Services.AddGrpc();

var app = builder.Build();

if (inMigratorMode)
{
    await ElasticMigrator.MigrateAsync(app);
    return;
}

app.MapGrpcServices();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();