using Adapters.PostgresSQL.Repository;
using Domain.Application.Service;
using Domain.Core.Contracts.Repositories;
using Domain.Core.Contracts.Services;
using Job;

namespace Infra.Registers
{
    public static class DomainExtensions
    {

        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Jobs

            //services.AddSingleton<IMonitorCargaJob, ObservadorCargaJob>();

            #endregion

            #region Services

            services.AddSingleton<IObservadorService, ObservadorService>();

            #endregion

            #region Repositories

            services.AddSingleton<IArquivoRepository, ArquivoRepository>();


            #endregion

            return services;
        }




    }
}
