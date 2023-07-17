using Adapters.Broker.Models;
using RabbitMQ.Client;

namespace Adapters.Broker.Connection
{
    public interface IRabbitMQConnection
    {
        bool IsConnected { get; }
        IConnection Connection { get; }

        RabbitMQSettings RabbitSettings { get; }
        void TryConnect();
        IModel CreateModel();
        void Dispose();

    }
}
