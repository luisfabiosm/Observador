using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapters.PostgresSQL.Settings
{
    public record PostgresSettings
    {
        public string Host { get;  set; }
        public string Database { get; set; }

        public string User { get; set; }
        public string Password { get; set; }
    }
}
