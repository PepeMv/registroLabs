using Registrov2._0.Models;
using Registrov2._0.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Controllers
{
    public class MateriaController : Controller
    {
        // GET: Materia
        public ActionResult Index()
        {
            //modelo que regresa
            MateriaModeloIndex modelo = new MateriaModeloIndex();

            List<SelectListItem> lst = new List<SelectListItem>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.periodo
                       select new SelectListItem
                       {
                           Value = d.id.ToString(),
                           Text = d.nombre,

                       }).ToList();
            }

            modelo.Periodos = lst;
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
        public JsonResult getMateriasxSemestre(int idSemestre)
        {
            //lista para devolver semestres
            List<MateriaDocenteModeloIndex> respuesta = new List<MateriaDocenteModeloIndex>();
            //List<MateriaVM> materiasIniciales = new List<MateriaVM>();

            using (registroLabsEntities db = new registroLabsEntities())
            {

                respuesta = (from m in db.materia
                                     where m.idSemestre == idSemestre
                             select new MateriaDocenteModeloIndex
                             {
                                 Id = m.id,
                                 Nombre = m.nombre,
                                Docente = null,
                                IdDocente = m.idDocente.HasValue ? (int)m.idDocente : 0

                             }).ToList();

                foreach (var item in respuesta)
                {
                    DocenteSimple docente = new DocenteSimple();

                    if (item.IdDocente != 0)
                    {
                        docente = (from d in db.docente
                                   where d.id == item.IdDocente
                                   select new DocenteSimple
                                   {
                                       Id = d.id,
                                       Nombre = d.nombre,
                                       Apellido = d.apellido

                                   }).FirstOrDefault();

                        if (docente != null)
                        {
                            item.Docente = docente.Nombre + " " + docente.Apellido;
                        }
                    }

                }
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Nuevo()
        {
            //modelo para regresar
            MateriaModeloNuevo modelo = new MateriaModeloNuevo();
            List<SelectListItem> lst = new List<SelectListItem>();
            List<SelectListItem> docentes = new List<SelectListItem>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.periodo
                       select new SelectListItem
                       {
                           Value = d.id.ToString(),
                           Text = d.nombre,

                       }).ToList();

                docentes = (from d in db.docente
                            select new SelectListItem
                            {
                                Value = d.id.ToString(),
                                Text = d.nombre +" "+ d.apellido,

                            }).ToList();
            }

            modelo.Periodos = lst;
            modelo.Docentes = docentes;
            modelo.InsertarMateria = new InsertarMateria();

            return View(modelo);
        }


        [HttpPost]
        public ActionResult Nuevo(MateriaModeloNuevo modelo)
        {

            try
            {
                if (modelo.InsertarMateria.Nombre != "" && modelo.InsertarMateria.IdSemestre.ToString() != null )
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {

                        {
                            var newMateria = new materia();
                            newMateria.nombre = modelo.InsertarMateria.Nombre;
                            newMateria.idDocente = modelo.InsertarMateria.IdDocente;
                            newMateria.idSemestre = modelo.InsertarMateria.IdSemestre;

                            db.materia.Add(newMateria);
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Materia ingresada";
                            ViewBag.exito = TempData["exito"];

                            return RedirectToAction("/");
                        }
                    }
                }
                else
                {
                    //enviar error
                    TempData["error"] = "Los datos ingresados son incorrectos!";
                    ViewBag.error = TempData["error"];

                    return RedirectToAction("/");
                }
            }
            catch
            {

                //enviar error
                TempData["error"] = "Intente mas tarde!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("/");
            }

        }


        public ActionResult Editar(int id)
        {
            MateriaModeloEditar modelo = new MateriaModeloEditar();

            InsertarMateria materiaEdi = new InsertarMateria();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var materiaTemp = db.materia.Find(id);
                materiaEdi.Id = materiaTemp.id;
                materiaEdi.Nombre = materiaTemp.nombre;
                materiaEdi.IdDocente = materiaTemp.idDocente.HasValue ? (int)materiaTemp.idDocente : 0;
                materiaEdi.IdSemestre = (int)materiaTemp.idSemestre;

            }

            List<SelectListItem> lst = new List<SelectListItem>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.periodo
                       select new SelectListItem
                       {
                           Value = d.id.ToString(),
                           Text = d.nombre,

                       }).ToList();
            }

            SemestreSimple semestre = new SemestreSimple();
            using (registroLabsEntities db = new registroLabsEntities())
            {
                semestre = (from d in db.semestre
                           where d.id == materiaEdi.IdSemestre
                           select new SemestreSimple
                           {
                               IdCarrera = (int)d.idCarrera

                           }).FirstOrDefault();
            }


            CarreraSimple carrera = new CarreraSimple();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                carrera = (from d in db.carrera
                           where d.id == semestre.IdCarrera
                           select new CarreraSimple
                           {
                               IdPerdiodo = (int)d.idPeriodo

                           }).FirstOrDefault();
            }

            List<SelectListItem> docentes = new List<SelectListItem>();
            using (registroLabsEntities db = new registroLabsEntities())
            {
                docentes = (from d in db.docente
                            select new SelectListItem
                            {
                                Value = d.id.ToString(),
                                Text = d.nombre + " " + d.apellido,

                            }).ToList();
            }

            modelo.InsertarMateria = materiaEdi;
            modelo.Periodos = lst;
            modelo.Docentes = docentes;
            modelo.IdPeriodo = carrera.IdPerdiodo;
            modelo.IdCarrera = semestre.IdCarrera;


            return View(modelo);
        }

        [HttpPost]
        public ActionResult Editar(MateriaModeloEditar modelo)
        {

            try
            {
                if (modelo.InsertarMateria.Nombre != "" && modelo.InsertarMateria.IdSemestre.ToString() != null )
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {

                        {
                            var newMateria = db.materia.Find(modelo.InsertarMateria.Id);
                            newMateria.nombre = modelo.InsertarMateria.Nombre;
                            newMateria.idSemestre = modelo.InsertarMateria.IdSemestre;
                            newMateria.idDocente = modelo.InsertarMateria.IdDocente;

                            db.Entry(newMateria).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Materia actualizado";
                            ViewBag.exito = TempData["exito"];

                            return RedirectToAction("/");
                        }
                    }
                }
                else
                {
                    TempData["error"] = "Algo salio mal intente nuevamente!";
                    ViewBag.error = TempData["error"];

                    return RedirectToAction("/");
                }
            }
            catch
            {

                //enviar error
                TempData["error"] = "Intente mas tarde!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("/");
            }

        }


        [HttpGet]
        public JsonResult Eliminar(int idMateria)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    materia materiaEliminar = db.materia.Find(idMateria);
                    db.materia.Remove(materiaEliminar);
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


        [HttpGet]
        public JsonResult getMateriaxDocente(int idDocente, string fechaIngreso)
        {
            DateTime fecha = new DateTime();

            if (fechaIngreso == null || fechaIngreso == "")
            {
                DateTime today = DateTime.Now;
                string fechaTemporal = today.ToString("yyyy-MM-dd");
                fecha = DateTime.ParseExact(fechaTemporal, "yyyy-MM-dd", null);
            }
            else
            {
                fecha = DateTime.ParseExact(fechaIngreso, "yyyy-MM-dd", null);
            }

            //variables iniciales para respuesta de consulta
            PeriodoSimple periodo = new PeriodoSimple();
            List<SelectListItem> lst = new List<SelectListItem>();


            
            

            using (registroLabsEntities db = new registroLabsEntities())
            {

                periodo = (from d in db.periodo
                           where fecha >= d.fechaInicio && fecha <= d.fechaFin
                           select new PeriodoSimple
                           {
                               Id = d.id,
                               Nombre = d.nombre,
                               FechaInicio = (DateTime)d.fechaInicio,
                               FechaFin = (DateTime)d.fechaFin
                           }).FirstOrDefault();

                if (periodo == null)
                {
                    JsonRespuesta rst = new JsonRespuesta();
                    rst.Status = "error";
                    rst.Mensaje = "No se encontro el periodo de la fecha indicada!";

                    return Json(rst, JsonRequestBehavior.AllowGet);
                }

                //variables temporales para el join de la consulta
                List<materia> materias = db.materia.ToList();
                List<semestre> semestres = db.semestre.ToList();
                List<carrera> carreras = db.carrera.ToList();

                lst = ( from m in materias
                        join s in semestres on m.idSemestre equals s.id into table1
                        from s in table1.ToList()
                        join c in carreras on s.idCarrera equals c.id into table2
                        from c in table2.ToList()
                        where c.idPeriodo == periodo.Id && m.idDocente ==idDocente
                        select new SelectListItem
                        {
                            Value = m.id.ToString(),
                            Text = m.nombre
                        }).ToList();

                //la lista no tiene algo  ?
                if (!lst.Any())
                {
                    JsonRespuesta rst = new JsonRespuesta();
                    rst.Status = "info";
                    rst.Mensaje = "No se encontraron materias asignadas al docente!";

                    return Json(rst, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        //concultar materia con el periodo como parametro
        [HttpGet]
        public JsonResult getMateriaxDocenteyPeriodo(int idDocente, int idPeriodo)
        {
            

            //variables iniciales para respuesta de consulta
            PeriodoSimple periodo = new PeriodoSimple();
            List<SelectListItem> lst = new List<SelectListItem>();





            using (registroLabsEntities db = new registroLabsEntities())
            {

                periodo = (from d in db.periodo
                           where d.id == idPeriodo
                           select new PeriodoSimple
                           {
                               Id = d.id,
                               Nombre = d.nombre,
                               FechaInicio = (DateTime)d.fechaInicio,
                               FechaFin = (DateTime)d.fechaFin
                           }).FirstOrDefault();

                if (periodo == null)
                {
                    JsonRespuesta rst = new JsonRespuesta();
                    rst.Status = "error";
                    rst.Mensaje = "No se encontro el periodo!";

                    return Json(rst, JsonRequestBehavior.AllowGet);
                }

                //variables temporales para el join de la consulta
                List<materia> materias = db.materia.ToList();
                List<semestre> semestres = db.semestre.ToList();
                List<carrera> carreras = db.carrera.ToList();

                lst = (from m in materias
                       join s in semestres on m.idSemestre equals s.id into table1
                       from s in table1.ToList()
                       join c in carreras on s.idCarrera equals c.id into table2
                       from c in table2.ToList()
                       where c.idPeriodo == periodo.Id && m.idDocente == idDocente
                       select new SelectListItem
                       {
                           Value = m.id.ToString(),
                           Text = m.nombre
                       }).ToList();

                //la lista no tiene algo  ?
                if (!lst.Any())
                {
                    JsonRespuesta rst = new JsonRespuesta();
                    rst.Status = "info";
                    rst.Mensaje = "No se encontraron materias asignadas al docente!";

                    return Json(rst, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}
