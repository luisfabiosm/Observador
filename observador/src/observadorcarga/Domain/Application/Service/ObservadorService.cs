
using Domain.Core.Contracts.Repositories;
using Domain.Core.Contracts.Services;
using System.Collections.Generic;


namespace Domain.Application.Service
{
    public  class ObservadorService : IObservadorService
    {
        private readonly IArquivoRepository _repo;
        private readonly ILogger<ObservadorService> _logger;

        public ObservadorService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ObservadorService>>();
            _repo = serviceProvider.GetRequiredService<IArquivoRepository>();
        }

        public async Task Processamento()
        {
            try
            {
                string data = DateTime.Now.ToString("yyyy-MM-dd");

                var list = await _repo.MonitorArquivosLiberados(data);

                Console.WriteLine(list);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
