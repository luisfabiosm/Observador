using Domain.Core.Models.SPA;


namespace Domain.Core.Contracts.Repositories
{
    public interface IArquivoRepository
    {
        Task<List<InterfaceArquivo>> MonitorArquivosLiberados(string dataInterface);

    }
}
