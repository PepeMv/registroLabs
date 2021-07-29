using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class ConsultaLaboratorioHorasViewModel
    {
        public int IdLabo { get; set; }
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime Fecha { get; set; }
    }
}
