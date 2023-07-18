

namespace Domain.Core.Models.SPA
{
    public record InterfaceArquivo
    {
        public string? Id { get; set; }
        public DateTime? DataInterface { get; set; }
        public string? Dependencia { get; set; }
        public bool? Liberado  { get; set; }

        ~InterfaceArquivo()
        {

            this.Id = null; 
            this.DataInterface = null; 
            this.Dependencia = null; 
            this.Liberado = null; 
        }
    }
}
