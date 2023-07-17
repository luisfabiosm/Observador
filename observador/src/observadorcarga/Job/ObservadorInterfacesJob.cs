using Domain.Core.Contracts.Services;
using Quartz;

namespace Job
{
    [DisallowConcurrentExecution]
    public class ObservadorInterfacesJob : IJob
    {
        private readonly ILogger<ObservadorInterfacesJob> _logger;
        private readonly IObservadorService _domainService;

        public ObservadorInterfacesJob(IServiceProvider  serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ObservadorInterfacesJob>>();
            _domainService = serviceProvider.GetRequiredService<IObservadorService>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($" Execute at: {DateTime.Now}.");
                Console.WriteLine($" Execute at: {DateTime.Now}.");

                _domainService.Processamento();

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
            }

        }
    }


}
