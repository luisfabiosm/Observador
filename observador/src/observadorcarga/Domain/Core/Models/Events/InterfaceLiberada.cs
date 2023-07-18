using Domain.Core.Models.SPA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.Models.Events
{
    public record InterfaceLiberada 
    {
        public string? Id { get; set; }
        public DateTime? DataInterface { get; set; }
        public string? Dependencia { get; set; }
        public bool? Liberado { get; set; }

        public DateTime? DataLiberacao { get; set; }

        public InterfaceLiberada()
        {
            
        }

        public InterfaceLiberada(InterfaceArquivo item)
        {
            this.Id = item.Id;
            this.DataInterface = item.DataInterface;
            this.Dependencia = item.Dependencia;
            this.Liberado = item.Liberado;
            this.DataLiberacao = DateTime.Now;
        }

        ~InterfaceLiberada()
        {

            this.Id = null;
            this.DataInterface = null;
            this.Dependencia = null;
            this.Liberado = null;
            DataLiberacao = null;
        }

    }
}
