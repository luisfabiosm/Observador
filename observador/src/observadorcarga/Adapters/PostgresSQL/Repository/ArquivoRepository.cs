using Adapters.PostgresSQL.Connection;
using Dapper;
using Domain.Core.Contracts.Repositories;
using Domain.Core.Models.Entity;
using Domain.Core.Models.SPA;
using System.Data;



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

            string command = "SELECT * FROM \"InterfaceArquivo\"  WHERE \"DataInterface\" = @DataInterface AND \"Liberado\" = true";
            return  (await _session.QueryAsync<InterfaceArquivo>(command, new { DataInterface = DateTime.Parse(dataInterface) })).ToList(); 

        }

        public async Task<InterfaceLog> InterfaceLogJaCarregada(InterfaceArquivo interfaceArquivo)
        {

            string command = "SELECT * FROM \"InterfaceLog\"  WHERE \"DataInterface\" = @DataInterface AND \"Id\" = @Id AND  \"Dependencia\" = @Dependencia AND \"Versao\" = @Versao AND \"Status\" = 1 ";
            return (await _session.QueryFirstOrDefaultAsync<InterfaceLog>(command, new
            {
                DataInterface = interfaceArquivo.DataInterface,
                Id = interfaceArquivo.Id,
                Dependencia = interfaceArquivo.Dependencia,
                Versao = 1
            }));
   
        }


        public async Task GravarLog(InterfaceArquivo interfaceArquivo)
        {
            string command = "INSERT INTO \"InterfaceLog\"  (\"Id\", \"DataInterface\", \"Dependencia\", \"DataLiberado\", \"Versao\", \"Status\"  ) " +
                " VALUES ( @Id, @DataInterface, @Dependencia, @DataLiberado, @Versao, @Status ) ";

            //string command = $"INSERT INTO \"InterfaceLog\"  (Id, DataInterface, Dependencia, DataLiberado, Versao, Status ) " +
            //    " VALUES ( @Id, @DataInterface, @Dependencia, @DataLiberado, @Versao, @Status ) ";

            var log = new
            {
                Id = interfaceArquivo.Id,
                DataInterface = interfaceArquivo.DataInterface,
                Dependencia = interfaceArquivo.Dependencia,
                DataLiberado = DateTime.Now,
                Versao = 1,
                Status = 1
            };
            await _session.ExecuteAsync(command, log);
        }

    }
}
