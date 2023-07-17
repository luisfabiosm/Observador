using Adapters.PostgresSQL.Connection;
using Dapper;
using Domain.Core.Contracts.Repositories;
using Domain.Core.Models.SPA;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace Adapters.PostgresSQL.Repository
{
    public class ArquivoRepository : IArquivoRepository
    {
        private readonly ISQLConnection _connection;
        private readonly IDbConnection _session;


        public ArquivoRepository(IServiceProvider serviceProvider)
        {
            _connection = serviceProvider.GetRequiredService<ISQLConnection>();
            _session = _connection.Connection();
        }

        public async Task<List<InterfaceArquivo>> MonitorArquivosLiberados(string dataInterface)
        {

            var command = $"SELECT * FROM public.\"InterfaceArquivo\"  where \"DataInterface\" = '@dataInterface' AND \"Liberado\" = @Liberado";
            return  (await _session.QueryAsync<InterfaceArquivo>(command, new { DataInterface = dataInterface, Liberado = true})).ToList();

        }


  
    }
}
