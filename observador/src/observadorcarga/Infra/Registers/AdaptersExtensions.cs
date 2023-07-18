
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

            services.Configure<RabbitMQSettings>(options => configuration.GetSection("Broker").Bind(options));

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();

                var settings = new RabbitMQSettings();
                configuration.GetSection("Broker").Bind(settings);
   
                ConnectionFactory factory = new()
                {
                    HostName = settings.Host,
                    Port = settings.Port,
                    UserName = settings.Username,
                    Password = settings.Password,
                    RequestedConnectionTimeout = TimeSpan.FromMilliseconds(Convert.ToDouble(settings.ConnectionTimeout))
                };

                return new RabbitMQConnection(factory, settings, logger);
            });

            services.AddSingleton<IBrokerPublishService, BrokerPublishService>();

            #endregion


            return services;
        }
    }
}
