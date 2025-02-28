using StorageService.Kafka;

var builder = WebApplication.CreateBuilder(args);

//validate whether services' lifetimes are valid and correspond to each other.
builder.WebHost.UseDefaultServiceProvider(
    (_, options) =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });

builder.Services.AddKafka();

var app = builder.Build();


app.Run();