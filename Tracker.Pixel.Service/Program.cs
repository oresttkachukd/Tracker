using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Tracker.Pixel.Service.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables(prefix: "TRACKER_");

var app = builder.Build();
app.MapGet("/track", async (HttpContext httpContext, IWebHostEnvironment environment) =>
{
    var referrer = httpContext.Request.Headers["Referer"].ToString();
    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

    httpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    httpContext.Response.Headers["Pragma"] = "no-cache";
    httpContext.Response.Headers["Expires"] = "0";

    var filePath = Path.Combine(environment.ContentRootPath, "Resources", "pix.gif");
    var fileBytes = FileCache.ReadFile(filePath);
    var file = Results.File(fileBytes, "image/gif");

    return file;
});

app.Run();
