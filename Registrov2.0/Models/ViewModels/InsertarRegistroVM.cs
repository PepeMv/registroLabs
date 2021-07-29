using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class InsertarRegistroVM
    {
        public int Id { get; set; }        
        //[DataType(DataType.Date)]
        [Required]
        public string Fecha { get; set; }
        [Required]
        [Display(Name = "Hora inicio")]
        public string horaInicio { get; set; }
        [Required]
        [Display(Name = "Hora fin")]
        public string horaFin { get; set; }
        [Required]
        public string Tema { get; set; }
        public int IdLaboratorio { get; set; }
        
        public string Observacion { get; set; }
        [Required]
        public int IdMateria { get; set; }

        public int idPeriodo { get; set; }

        public string DiaSemana { get; set; }
    }
}
