using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Models.ViewModels
{
    public class SemestreModeloNuevo
    {
        public InsertarSemestre InsertarSemestre { get; set; }
        public List<SelectListItem> Periodos { get; set; }
    }
}
