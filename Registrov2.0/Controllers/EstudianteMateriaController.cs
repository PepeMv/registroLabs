using Registrov2._0.Models;
using Registrov2._0.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Controllers
{
    public class EstudianteMateriaController : Controller
    {
        // GET: EstudianteMateria
        public ActionResult Index()
        {
            //modelo que regresa
            EstudianteMateriaModeloIndex modelo = new EstudianteMateriaModeloIndex();

            List<SelectListItem> lstPeriodos = new List<SelectListItem>();
            List<SelectListItem> lstEstudiantes = new List<SelectListItem>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lstPeriodos = (from d in db.periodo
                               select new SelectListItem
                               {
                                   Value = d.id.ToString(),
                                   Text = d.nombre,

                               }).ToList();
            }

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lstEstudiantes = (from d in db.estudiante
                                  select new SelectListItem
                                  {
                                      Value = d.id.ToString(),
                                      Text = d.nombre,

                                  }).ToList();
            }

            modelo.Periodos = lstPeriodos;
            modelo.Estudiantes = lstEstudiantes;

            try
            {
                ViewBag.exito = TempData["exito"].ToString();
            }
            catch { }

            try
            {
                ViewBag.error = TempData["error"].ToString();
            }
            catch { }

            return View(modelo);
        }

        [HttpGet]
        public JsonResult getMateriasxEstudianteyPeriodo(int idEstudiante, int idPeriodo)
        {
            //lista para devolver la materia el semestre y el id del registro
            List<EstudianteMateriaJoin> respuesta = new List<EstudianteMateriaJoin>();


            using (registroLabsEntities db = new registroLabsEntities())
            {
                List<periodo> periodos = db.periodo.ToList();
                List<carrera> carreras = db.carrera.ToList();
                List<semestre> semestres = db.semestre.ToList();
                List<materia> materias = db.materia.ToList();
                List<detalle_materia> detallesMateria = db.detalle_materia.ToList();


                respuesta = (from p in periodos
                             join c in carreras on p.id equals c.idPeriodo into table1
                             from c in table1.ToList()
                             join s in semestres on c.id equals s.idCarrera into table2
                             from s in table2.ToList()
                             join m in materias on s.id equals m.idSemestre into table3
                             from m in table3.ToList()
                             join dm in detallesMateria on m.id equals dm.idMateria into table4
                             from dm in table4.ToList()
                             where dm.idEstudiante == idEstudiante && p.id == idPeriodo
                             select new EstudianteMateriaJoin
                             {
                                 NombreMateria = m.nombre,
                                 NombreSemestre = s.nombre,
                                 idDetalleMateria = dm.id,

                             }).ToList();


            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult insertarMaterialEstudiante(int idMateria, int idEstudiante)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            //variable para ver si el estudiante ya tiene registrada esa materia
            DetalleMateriaSimple detalleMateria = new DetalleMateriaSimple();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                detalleMateria = (from dm in db.detalle_materia
                                  where dm.idMateria == idMateria && dm.idEstudiante == idEstudiante
                                  select new DetalleMateriaSimple
                                  {
                                      Id = dm.id
                                  }).FirstOrDefault();

                if (detalleMateria != null)
                {
                    respuesta.Status = "error";
                    respuesta.Mensaje = "El estudiante ya tiene asignada la materia!";

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }


                try
                {
                    var newDetalleMateria = new detalle_materia();
                    newDetalleMateria.idEstudiante = idEstudiante;
                    newDetalleMateria.idMateria = idMateria;
                    

                    db.detalle_materia.Add(newDetalleMateria);
                    db.SaveChanges();

                    respuesta.Status = "success";
                    respuesta.Mensaje = "La materia se agrego correctamente!";

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    respuesta.Status = "error";
                    respuesta.Mensaje = "Intente nuevamente!";

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }

            }
        }

        [HttpGet]
        public JsonResult EliminarMateriaEstudiante(int idDetalleMateria)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    detalle_materia materiaDetalleEliminar = db.detalle_materia.Find(idDetalleMateria);
                    db.detalle_materia.Remove(materiaDetalleEliminar);
                    db.SaveChanges();

                    respuesta.Status = "success";
                    respuesta.Mensaje = "Registro eliminado correctamente!";

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    respuesta.Status = "error";
                    respuesta.Mensaje = "El registro no se elimino porque ya esta referenciado!";

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }

            }
        }
    }
}
