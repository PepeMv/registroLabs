using Registrov2._0.Models;
using Registrov2._0.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Controllers
{
    public class SemestreController : Controller
    {
        // GET: Semestre
        public ActionResult Index()
        {
            //modelo que regresa
            SemestreModeloIndex modelo = new SemestreModeloIndex();

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
        public JsonResult getSemestresxCarrera(int idCarrera)
        {
            //lista para devolver semestres
            List<SemestreSimple> semestres = new List<SemestreSimple>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                semestres = (from s in db.semestre
                            where s.idCarrera == idCarrera
                            select new SemestreSimple
                            {
                                Id = s.id,
                                Nombre = s.nombre,
                                IdCarrera = (int)s.idCarrera

                            }).ToList();
            }

            return Json(semestres, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Nuevo()
        {
            //modelo para regresar
            SemestreModeloNuevo modelo = new SemestreModeloNuevo();
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
            modelo.InsertarSemestre = new InsertarSemestre();

            return View(modelo);
        }

        [HttpPost]
        public ActionResult Nuevo(SemestreModeloNuevo modelo)
        {

            try
            {
                if (modelo.InsertarSemestre.Nombre != "" && modelo.InsertarSemestre.IdCarrera.ToString() != null)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {

                        {
                            var newSemestre = new semestre();
                            newSemestre.nombre = modelo.InsertarSemestre.Nombre;
                            newSemestre.idCarrera = modelo.InsertarSemestre.IdCarrera;

                            db.semestre.Add(newSemestre);
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Semestre ingresado";
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
            SemestreModeloEditar modelo = new SemestreModeloEditar();

            InsertarSemestre semestre = new InsertarSemestre();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var semestreTemp = db.semestre.Find(id);
                semestre.Id = semestreTemp.id;
                semestre.Nombre = semestreTemp.nombre;
                semestre.IdCarrera = (int)semestreTemp.idCarrera;

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


            modelo.InsertarSemestre = semestre;
            modelo.Periodos = lst;
            modelo.IdPeriodo = carrera.IdPerdiodo;


            return View(modelo);
        }

        [HttpPost]
        public ActionResult Editar(SemestreModeloEditar modelo)
        {

            try
            {
                if (modelo.InsertarSemestre.Nombre != "" && modelo.InsertarSemestre.IdCarrera.ToString() != null && modelo.InsertarSemestre.Id.ToString() != null)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {

                        {
                            var newSemestre = db.semestre.Find(modelo.InsertarSemestre.Id);
                            newSemestre.nombre = modelo.InsertarSemestre.Nombre;
                            newSemestre.idCarrera = modelo.InsertarSemestre.IdCarrera;

                            db.Entry(newSemestre).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Semestre actualizado";
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
        public JsonResult Eliminar(int idSemestre)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    semestre semestreEliminar = db.semestre.Find(idSemestre);
                    db.semestre.Remove(semestreEliminar);
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
