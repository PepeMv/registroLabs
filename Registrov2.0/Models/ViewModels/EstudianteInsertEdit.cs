using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class EstudianteInsertEdit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cedula de Identidad")]
        public string Cedula { get; set; }

        [Required]
        [Display(Name = "Nombres Estudiante")]
        public string Nombre { get; set; }

        

    }
}
