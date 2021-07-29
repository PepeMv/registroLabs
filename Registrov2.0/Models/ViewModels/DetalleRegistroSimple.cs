using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class DetalleRegistroSimple
    {
        public int Id { get; set; }
        public int IdRegistro { get; set; }
        public int IdEstudianteDocente { get; set; }
        public string Observacion { get; set; }
    }
}
