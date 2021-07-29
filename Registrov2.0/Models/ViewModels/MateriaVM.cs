using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class MateriaVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdDocente { get; set; }
        public int IdSemestre { get; set; }
    }
}
