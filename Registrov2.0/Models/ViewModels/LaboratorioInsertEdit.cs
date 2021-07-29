using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class LaboratorioInsertEdit
    {
        public int Id { get; set; }

        
        [Required]
        [Display(Name = "Nombre Laboratorio")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Capacidad Laboratorio")]
        public int Capacidad { get; set; }
        
        [Required]
        [Display(Name = "Subred Laboratorio")]
        public int Subred { get; set; }


        

    }
}
