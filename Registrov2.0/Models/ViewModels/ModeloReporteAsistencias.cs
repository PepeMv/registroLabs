using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registrov2._0.Models.ViewModels
{
    public class ModeloReporteAsistencias
    {
        public RegistroSimpleVM Registro { get; set; }
        public MateriaVM Materia { get; set; }
        public DocenteSimple Docente { get; set; }
        public SemestreSimple Semestre { get; set; }
        public CarreraSimple Carrera { get; set; }
        public PeriodoSimple Periodo { get; set; }
        public List<DetalleRegistroEstudianteInfo> Estudiantes { get; set; }
        public LaboratorioSimple Laboratorio { get; set; }
    }
}
