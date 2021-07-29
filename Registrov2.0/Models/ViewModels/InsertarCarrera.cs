using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class InsertarCarrera
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nombre de la carrera")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Periodo")]
        public int IdPeriodo { get; set; }

    }
}
