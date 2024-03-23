using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tracker.Storage.Infrastructure.EventStore;
using Tracker.Storage.Application.Abstractions;

namespace Tracker.Storage.Infrastructure
{
    public static class IoCConfig
    {
        public static IServiceCollection AddFileEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("FileEventStore").Get<FileEventStoreSettings>() ?? new FileEventStoreSettings();

            services.AddSingleton(settings);
            services.AddSingleton<IEventStore, FileEventStore>();

            return services;
        }
    }
}
