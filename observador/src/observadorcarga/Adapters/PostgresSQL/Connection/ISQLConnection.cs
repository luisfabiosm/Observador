using Adapters.PostgresSQL.Settings;
using System.Data;


namespace Adapters.PostgresSQL.Connection
{
    public interface ISQLConnection
    {
        IDbConnection Connection();


    }
}
