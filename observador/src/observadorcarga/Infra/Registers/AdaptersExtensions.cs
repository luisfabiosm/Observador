
using Adapters.Broker.Connection;
using RabbitMQ.Client;
using Adapters.Broker.Models;
using Domain.Core.Contracts.Services;
using Adapters.Broker.Services;
using Adapters.PostgresSQL.Connection;
using Adapters.PostgresSQL.Settings;

namespace Infra.Registers
{
    public static class AdaptersExtensions
    {
        public static IServiceCollection AddAdapters(this IServiceCollection services, IConfiguration configuration)
        {


            #region SQL Session Management
            services.AddSingleton<ISQLConnection>(options =>
            {
                var settings = new PostgresSettings();
                configuration.GetSection("PostgresSQL").Bind(settings);
                ILogger<SQLConnection> _logger = options.GetRequiredService<ILogger<SQLConnection>>();
                return new SQLConnection(_logger, settings);

             });

            //services.AddSingleton<ISQLConnection, SQLConnection>(x => new SQLConnection(x, configuration, new[] { "" }));

            #endregion


            #region RabbitMQ

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();

                RabbitMQSettings credentials = new()
                {
                    Host = configuration.GetSection("EventBusSettings")["Hostname"],
                    Port = int.Parse(configuration.GetSection("EventBusSettings")["Port"] ?? "5672"),
                    Username = configuration.GetSection("EventBusSettings")["Username"],
                    Password = configuration.GetSection("EventBusSettings")["Password"],
                    Queue = configuration.GetSection("RabbitMQ")["Queue"],
                    Exchange = configuration.GetSection("RabbitMQ")["Exchange"],
                    Retry = int.Parse(configuration.GetSection("RabbitMQ")["Retry"]),
                    Delay = int.Parse(configuration.GetSection("RabbitMQ")["Delay"]),
                    ConnectionTimeout = int.Parse(configuration.GetSection("EventBusSettings")["ConnectionTimeout"]),
                    MessageLimit = int.Parse(configuration.GetSection("RabbitMQ")["MessageLimit"]),
                };


                ConnectionFactory factory = new()
                {
                    HostName = credentials.Host,
                    Port = credentials.Port,
                    UserName = credentials.Username,
                    Password = credentials.Password,
                    RequestedConnectionTimeout = TimeSpan.FromMilliseconds(Convert.ToDouble(credentials.ConnectionTimeout))
                };

                return new RabbitMQConnection(factory, credentials, logger);
            });

            services.AddScoped<IBrokerPublishService, BrokerPublishService>();

            #endregion


            return services;
        }
    }
}
