using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class ListPeriodos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
