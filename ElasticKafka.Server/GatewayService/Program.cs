using GatewayService.Extensions;
using GatewayService.Kafka;
using GatewayService.Messages.RemoteServiceDiscovery;
using GatewayService.Messages.Services;
using GatewayService.SignalR;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

//validate whether services' lifetimes are valid and correspond to each other.
builder.WebHost.UseDefaultServiceProvider(
    (_, options) =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });

builder.Services.AddControllers();

builder.Services
    .AddMessageServices()
    .AddKafka()
    .AddCustomSignalR(builder.Configuration)
    .AddSwagger()
    .AddCorsDefaultPolicy(builder.Configuration)
    .AddServiceDiscovery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseCustomSignalR();

app.UseCors();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapControllers();

app.Run();
