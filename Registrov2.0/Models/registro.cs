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
    
    public partial class registro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public registro()
        {
            this.detalle_registro = new HashSet<detalle_registro>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFin { get; set; }
        public string tema { get; set; }
        public Nullable<int> idLaboratorio { get; set; }
        public Nullable<int> idMateria { get; set; }
        public string observacion { get; set; }
    
        public virtual laboratorio laboratorio { get; set; }
        public virtual materia materia { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<detalle_registro> detalle_registro { get; set; }
    }
}
