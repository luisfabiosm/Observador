using Domain.Core.Models.Entity;
using Domain.Core.Models.Events;
using Domain.Core.Models.SPA;


namespace Domain.Core.Contracts.Repositories
{
    public interface IArquivoRepository
    {
        Task<List<InterfaceArquivo>> MonitorArquivosLiberados(string dataInterface);

        Task<InterfaceLog> InterfaceLogJaCarregada(InterfaceArquivo interfaceArquivo);

        Task GravarLog(InterfaceArquivo interfaceLiberada);

    }
}
