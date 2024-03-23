using System.Threading.Tasks;

namespace Tracker.Storage.Application.Abstractions
{
    public interface IEventStore
    {
        public Task Store(UserVisitedEventDto message); 
    }
}
