//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Registrov2._0.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class detalle_registro
    {
        public int id { get; set; }
        public Nullable<int> idRegistro { get; set; }
        public string observacion { get; set; }
        public Nullable<int> idEstudiante { get; set; }
        public Nullable<int> idDocente { get; set; }
    
        public virtual docente docente { get; set; }
        public virtual estudiante estudiante { get; set; }
        public virtual registro registro { get; set; }
    }
}
