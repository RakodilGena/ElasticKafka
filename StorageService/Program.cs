using StorageService.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafka();

var app = builder.Build();


app.Run();