using System;
using System.IO;
using System.Threading.Tasks;
using Tracker.Storage.Application.Abstractions;

namespace Tracker.Storage.Infrastructure.EventStore
{
    internal class FileEventStore : IEventStore, IDisposable
    {
        private StreamWriter _writer;

        public FileEventStore(FileEventStoreSettings settings)
        {
            if (!File.Exists(settings.File))
            {
                File.Create(settings.File).Close();
            }

            _writer = File.AppendText(settings.File);
        }

        public Task Store(UserVisitedEventDto message)
        {
            _writer.WriteLine($"{message.Timestamp.ToString("o")}|{message.Referrer ?? "null"}|{message.UserAgent ?? "null"}|{message.IpAddress ?? "null"}");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
