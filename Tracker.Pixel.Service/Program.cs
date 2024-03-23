using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    var referrer = httpContext.Request.Headers["Referrer"].ToString();
    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

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
