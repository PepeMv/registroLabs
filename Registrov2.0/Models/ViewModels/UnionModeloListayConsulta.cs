using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Models.ViewModels
{
    public class UnionModeloListayConsulta
    {
        public List<SelectListItem> ListaLaboratorios { get; set; }
        public ConsultaLaboratorioHorasViewModel Consulta { get; set; }

        public InsertarRegistroVM Registro { get; set; }

        public List<SelectListItem> ListaMaterias { get; set; }

        public List<SelectListItem> ListaDocentes { get; set; }

        public List<SelectListItem> ListaDias { get; set; }

        public List<SelectListItem> ListaPeriodos { get; set; }
        
    }
}
