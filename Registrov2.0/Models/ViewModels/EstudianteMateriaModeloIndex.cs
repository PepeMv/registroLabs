using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Models.ViewModels
{
    public class EstudianteMateriaModeloIndex
    {
        public List<SelectListItem> Periodos { get; set; }
        public List<SelectListItem> Estudiantes { get; set; }

    }
}
