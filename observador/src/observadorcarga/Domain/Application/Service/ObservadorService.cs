
using Adapters.Broker.Models;
using Domain.Core.Contracts.Repositories;
using Domain.Core.Contracts.Services;
using Domain.Core.Models.Events;
using Domain.Core.Models.SPA;
using Microsoft.Extensions.Options;
using System.Collections.Generic;


namespace Domain.Application.Service
{
    public  class ObservadorService : IObservadorService
    {
        private readonly IArquivoRepository _repo;
        private readonly IBrokerPublishService _brokerService;
        private readonly ILogger<ObservadorService> _logger;
        private readonly IOptions<RabbitMQSettings> _rabbitSettings;

        public ObservadorService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ObservadorService>>();
            _repo = serviceProvider.GetRequiredService<IArquivoRepository>();
            _brokerService = serviceProvider.GetRequiredService<IBrokerPublishService>();
            _rabbitSettings = serviceProvider.GetRequiredService<IOptions<RabbitMQSettings>>();
        }

        public async Task Processamento()
        {
            try
            {
                string data = DateTime.Now.ToShortDateString();
                var list = await _repo.MonitorArquivosLiberados(data);

                foreach (var item in list)
                {
                    Console.WriteLine(item);
                    await InformarLiberadaAsync(item);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private async Task InformarLiberadaAsync(InterfaceArquivo interfaceArquivo)
        {
            var log = await _repo.InterfaceLogJaCarregada(interfaceArquivo);

            if (log != null)
                return;

            var queue = "Interface" + interfaceArquivo.Id.PadLeft(4, '0') + ".To." + _rabbitSettings.Value.Queue;

            _brokerService.Publish(interfaceArquivo, queue, _rabbitSettings.Value.Exchange, interfaceArquivo.Id);
            await GravarLog(interfaceArquivo);
        }

        private async Task GravarLog(InterfaceArquivo interfaceLiberada)
        {
            await _repo.GravarLog(interfaceLiberada);
        }
    }
}
