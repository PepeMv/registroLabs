using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class DocenteInsertEdit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cedula de Identidad")]
        public string Cedula { get; set; }

        [Required]
        [Display(Name = "Nombres Docente")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellidos Docente")]
        public string Apellido { get; set; }


    }
}
