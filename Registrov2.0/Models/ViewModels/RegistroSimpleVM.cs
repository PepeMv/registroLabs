using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class RegistroSimpleVM
    {
        public int Id { get; set; }        
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Tema { get; set; }
        public int IdLaboratorio { get; set; }
        public string Observacion { get; set; }
        public int IdMateria { get; set; }
    }
}
