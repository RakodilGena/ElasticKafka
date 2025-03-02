using StorageService.Elastic;
using StorageService.Kafka;
using StorageService.Migration;

var builder = WebApplication.CreateBuilder(args);

var inMigratorMode = builder.Configuration.InMigratorMode();

//validate whether services' lifetimes are valid and correspond to each other.
builder.WebHost.UseDefaultServiceProvider(
    (_, options) =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });

builder.Services
    .AddKafka(inMigratorMode)
    .AddElastic(builder.Configuration);

var app = builder.Build();

if (inMigratorMode)
{
    await ElasticMigrator.MigrateAsync(app);
    return;
}

app.Run();

