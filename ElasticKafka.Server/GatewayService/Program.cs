using GatewayService.Extensions;
using GatewayService.Kafka;
using GatewayService.NewMessages.Services;
using GatewayService.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddMessageServices()
    .AddKafka()
    .AddCustomSignalR(builder.Configuration)
    .AddSwagger();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseCustomSignalR();

app.MapControllers();

app.Run();
