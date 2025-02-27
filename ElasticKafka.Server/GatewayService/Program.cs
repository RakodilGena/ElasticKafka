using GatewayService.Extensions;
using GatewayService.Kafka;
using GatewayService.NewMessages.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMessageServices();

builder.Services.AddKafka();

builder.AddSwagger();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
