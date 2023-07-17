using Adapters.Broker.Connection;
using Adapters.Broker.Models;
using Domain.Core.Contracts.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Adapters.Broker.Services
{
    public class BrokerPublishService : IBrokerPublishService
    {
        private readonly IRabbitMQConnection _rabbitConnection;
        internal IModel _channel;
        internal IConnection _connection;
        internal bool _disposed;
        internal RabbitMQSettings _settings;


        public BrokerPublishService(IRabbitMQConnection rabbitConnection)
        {
            _rabbitConnection = rabbitConnection;
            _connection = _rabbitConnection.Connection;
            _channel = _rabbitConnection.CreateModel();
            _settings = _rabbitConnection.RabbitSettings;

        }

        public void Publish<T>(T @object, string Queue = null, string Exchange = null, string RoutingKey = null)
        {

            try
            {
                _channel.BasicQos(0, 1, true);
                _channel.ExchangeDeclare(Exchange ?? _settings.Exchange, ExchangeType.Direct);

                _channel.QueueDeclare(
                    queue: Queue ?? _settings.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _channel.QueueBind(Queue ?? _settings.Queue, Exchange ?? _settings.Exchange, RoutingKey ?? "directexchange_key");

                IBasicProperties properties = _channel.CreateBasicProperties();
                byte[] messageBytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(@object));

                _channel.BasicPublish(Exchange ?? _settings.Exchange, RoutingKey ?? "directexchange_key", properties, messageBytes);
            }
            catch (Exception e)
            {
                throw new Exception($"RABBITMQ{DateTime.Now:ddMMyyyy} ERRO[Publish]: {e.Message}");
            }

        }


        public void Publish<T>(T @object, string Queue, string Exchange, string RoutingKey = null, bool deadLetters = false)
        {
            try
            {

                //dead letters
                string _deadletterExchange = $"{Exchange}-DeadLetterExchange";
                string _deadletterRoute = RoutingKey is null ? $"{Queue}-DeadLetterRouteKey" : $"{RoutingKey ?? Queue}-DeadLetterRouteKey";

                var _arguments = deadLetters == true ? new Dictionary<string, object>()
                {
                    {"x-dead-letter-exchange",_deadletterExchange },
                    {"x-dead-letter-routing-key",_deadletterRoute },
                } : null;

                _channel.ExchangeDeclare(_deadletterExchange, "direct", durable: true, autoDelete: false, arguments: _arguments);

                //Normal Exchange and Queue
                _channel.ExchangeDeclare(Exchange, "direct", durable: true, autoDelete: false);

                _channel.QueueDeclare(
                       queue: Queue,
                       durable: true,
                       exclusive: false,
                       autoDelete: false,
                       arguments: _arguments);
                _channel.QueueBind(Queue, Exchange, RoutingKey);

                _channel.BasicQos(0, 1, true);

                IBasicProperties properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                _channel.BasicPublish(exchange: Exchange,
                                     routingKey: RoutingKey,
                                     basicProperties: properties,
                                     body: Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(@object)));
            }
            catch (Exception e)
            {
                throw new Exception($"RABBITMQ{DateTime.Now:ddMMyyyy} ERRO[Publish]: {e.Message}");
            }



        }


        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                //_logger.LogCritical(ex.ToString());
            }
        }




    }
}
