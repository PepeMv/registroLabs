using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class InsertarSemestre
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nombre del Semestre")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Carrera")]
        public int IdCarrera { get; set; }
    }
}
