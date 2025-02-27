using GatewayService.Extensions;
using GatewayService.Kafka;
using GatewayService.NewMessages.Services;
using GatewayService.SignalR;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddMessageServices()
    .AddKafka()
    .AddCustomSignalR(builder.Configuration)
    .AddSwagger()
    .AddCorsDefaultPolicy(builder.Configuration);


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
