using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class ModeloVerificacionAsistencia
    {
        public int IdMateria { get; set; }
        public string NombreMateria { get; set; }
        public string NombreSemestre { get; set; }
        public string TipoUsuario { get; set; }
        public int IdUsuario { get; set; }
        public int IdRegistro { get; set; }
        public string NombreUsuario { get; set; }
        public string TemaRegistro { get; set; }
    }
}
