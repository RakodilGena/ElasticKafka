using MessagingService.Grpc;
using MessagingService.Kafka;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(x =>
        x.Protocols = HttpProtocols.Http2);
});

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddKafka();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcServices();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();