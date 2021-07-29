using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class RegistroVM
    {
        public int id { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string fechaInicio { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string fechaFin { get; set; }
        public string tema { get; set; }
        public int idLaboratorio { get; set; }
        public string observacion { get; set; }
        public int idMateria { get; set; }
        public string nombreMateria { get; set; }
        public string nombreDocente { get; set; }
    }
}
