using Domain.Core.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.Models.Entity
{
    public record InterfaceLog : InterfaceLiberada
    {
        private int _versao;
        private int _status;


        public DateTime DataLiberado { get; } = DateTime.Now;
        public int? Versao { get => _versao; }
        public int? Status { get=> _status; }


        public InterfaceLog()
        {


        }


        public InterfaceLog(InterfaceLiberada item)
        {
            _versao = 1;
            _status = 0;
            this.Id = item.Id;
            this.DataInterface = item.DataInterface;
            this.Dependencia = item.Dependencia;
            this.DataLiberacao = item.DataLiberacao;
            this.Liberado = item.Liberado;

        }

        public void NovaVersao(int versao)
        {
            _versao = versao;
        }

        public void NovoStatus(int versao)
        {
            _versao = versao;
        }

        ~InterfaceLog()
        {

        }
    }
}
