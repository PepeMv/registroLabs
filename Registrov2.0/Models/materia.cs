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
    
    public partial class materia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public materia()
        {
            this.detalle_materia = new HashSet<detalle_materia>();
            this.registro = new HashSet<registro>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public Nullable<int> idDocente { get; set; }
        public Nullable<int> idSemestre { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<detalle_materia> detalle_materia { get; set; }
        public virtual semestre semestre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<registro> registro { get; set; }
        public virtual docente docente { get; set; }
    }
}
