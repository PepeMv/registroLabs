using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Models.ViewModels
{
    public class MateriaModeloEditar
    {
        public InsertarMateria InsertarMateria{ get; set; }
        public List<SelectListItem> Periodos { get; set; }
        public List<SelectListItem> Docentes { get; set; }

        public int IdPeriodo { get; set; }
        public int IdCarrera { get; set; }
    }
}
