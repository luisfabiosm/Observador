using Adapters.Broker.Models;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace Adapters.Broker.Connection
{
    public class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _settings;
        private readonly ILogger _logger;

        internal IModel Channel;

        public int _retryCount = 5;
        public bool _disposed;


        public event Action<dynamic, ulong> OnReceiveMessage;
        public event Action<Exception, ulong> OnReceiveMessageException;
        public IConnection _connection;

        public bool IsConnected
        {
            get
            {
                return Connection != null && Connection.IsOpen && !_disposed;
            }
        }

        public IConnection Connection => _connection;

        public RabbitMQSettings RabbitSettings => _settings;

        public RabbitMQConnection(IConnectionFactory connectionFactory, RabbitMQSettings settings, ILogger<RabbitMQConnection> logger)
        {
            _logger = logger;
            _settings = settings;
            _connectionFactory = connectionFactory;
            _settings = settings;

            if (!IsConnected) TryConnect();
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("[EventBus] Conexao RabbitMQ indisponivel. Servico nao habilitado para transacionamento.");
            }
            return Connection.CreateModel();
        }

        public void TryConnect()
        {
            _logger.LogInformation("[EventBus] RabbitMQ Client tentando conectar...");

            var policy = Policy.Handle<SocketException>()
                                    .Or<BrokerUnreachableException>()
                                    .Or<Exception>()
                                    .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(_settings.Delay), (ex, time) =>
                                    {
                                        _logger.LogWarning($"[EventBus] RabbitMQ Client nao conectou depois de Timeout {time.TotalSeconds:n1}s ERRO: {ex.Message} IP {_settings.Host}:{_settings.Port}");
                                    });

            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();
            });

            if (IsConnected)
            {
                Connection.ConnectionShutdown += OnConnectionShutdown;
                Connection.CallbackException += OnCallbackException;
                Connection.ConnectionBlocked += OnConnectionBlocked;

                _logger.LogInformation("------------------------------------------------------");
                _logger.LogInformation($"[EventBus] RabbitMQ Client conectado ao Servidor: {_connection.Endpoint.HostName}");
                _logger.LogInformation("------------------------------------------------------");
            }
            else
            {
                _logger.LogWarning("------------------------------------------------------");
                _logger.LogWarning($"[EventBus] Conexao RabbitMQ nao pode ser estabelecida");
                _logger.LogWarning("------------------------------------------------------");
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("[EventBus] Conexao RabbitMQ finalizada. Tentando reconectar...");

            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("[EventBus] Conexao RabbitMQ error. Tentando reconectar....");

            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("[EventBus] Conexao RabbitMQ finalizada. Tentando reconectar...");

            TryConnect();
        }



        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                Connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}


