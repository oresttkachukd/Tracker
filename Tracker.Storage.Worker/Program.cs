using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tracker.BuildingBlocks.MessageHub.Abstractions;
using Tracker.BuildingBlocks.MessageHub.Kafka;
using Tracker.Pixel.Contracts;
using Tracker.Storage.Application.Abstractions;
using Tracker.Storage.Infrastructure;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureHostConfiguration(x =>
    x.AddJsonFile("appsettings.json")
    .AddEnvironmentVariables(prefix: "TRACKER:"));
builder
    .ConfigureServices((context, services) =>
    {
        services.AddKafkaMessageHub(context.Configuration);
        services.AddFileEventStore(context.Configuration);
    });

var app = builder.Build();

var hubConsumer = app.Services.GetService<IHubConsumer>();
var eventStore = app.Services.GetService<IEventStore>();
hubConsumer.Start<UserVisitedEvent>("storage-worker", Topics.UserEvents, x =>
{
    if (string.IsNullOrEmpty(x.IpAddress))
    {
        return Task.CompletedTask;
    }

    return eventStore.Store(new UserVisitedEventDto
    {
        Timestamp = x.Timestamp,
        IpAddress = x.IpAddress,
        Referrer = x.Referrer,
        UserAgent = x.UserAgent
    });
});

await app.RunAsync();

hubConsumer.Stop();