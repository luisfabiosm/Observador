

using Job;

namespace Infra.Registers
{
    public static class WorkerNativeInjection
    {
        public static IServiceCollection AddServiceInjections(this IServiceCollection services, IConfiguration configuration)
        {
          
            services.AddAdapters(configuration);
            services.AddDomainServices(configuration);

            return services;
        }
    }
}