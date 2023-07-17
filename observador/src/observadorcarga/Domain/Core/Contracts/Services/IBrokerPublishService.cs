

namespace Domain.Core.Contracts.Services
{
    public interface IBrokerPublishService
    {
        void Publish<T>(T @object, string Queue, string Exchange = null, string RoutingKey = null);

        void Publish<T>(T @object, string Queue, string Exchange, string RoutingKey = null, bool UseDeadLetters = false);

        void Dispose();
    }
}
