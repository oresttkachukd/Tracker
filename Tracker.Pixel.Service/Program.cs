using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Tracker.BuildingBlocks.MessageHub.Abstractions;
using Tracker.BuildingBlocks.MessageHub.Kafka;
using Tracker.Pixel.Contracts;
using Tracker.Pixel.Service.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables(prefix: "TRACKER:");
builder.Host
   .ConfigureServices((context, services) =>
    {
        services.AddKafkaMessageHub(context.Configuration);
        services.AddCors(options =>
        {
            options.AddPolicy("All", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
    });

var app = builder.Build();
app.UseCors("All");

app.MapGet("/track", (HttpContext httpContext, IWebHostEnvironment environment, IHubProducer producer) =>
{
    var referrer = httpContext.Request.Headers.TryGetValue(HeaderNames.Referer, out var referrerHeader) ? referrerHeader.First().ToString() : null;
    var userAgent = httpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgentHeader) ? userAgentHeader.First().ToString() : null;
    var ipAddress = httpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();

    httpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    httpContext.Response.Headers["Pragma"] = "no-cache";
    httpContext.Response.Headers["Expires"] = "0";

    producer.Produce(Topics.UserEvents, new UserVisitedEvent
    {
        Referrer = referrer,
        UserAgent = userAgent,
        IpAddress = ipAddress
    });

    var filePath = Path.Combine(environment.ContentRootPath, "Resources", "pix.gif");
    var fileBytes = FileCache.ReadFile(filePath);
    var file = Results.File(fileBytes, "image/gif");

    return file;
});

app.Run();
