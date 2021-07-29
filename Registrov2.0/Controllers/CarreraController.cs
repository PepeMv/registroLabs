using Registrov2._0.Models;
using Registrov2._0.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Controllers
{
    public class CarreraController : Controller
    {
        // GET: Carrera
        public ActionResult Index()
        {
            //modelo que regresa
            CarreraModeloIndex modelo = new CarreraModeloIndex();

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
        public JsonResult getCarrerasPorPeriodo(int idPeriodo)
        {
            //lista para devolver carreras
            List<CarreraSimple> carreras = new List<CarreraSimple>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                carreras = (from c in db.carrera
                            where c.idPeriodo == idPeriodo
                            select new CarreraSimple
                            {
                                Id = c.id,
                                Nombre = c.nombre,
                                IdPerdiodo = (int)c.idPeriodo

                            }).ToList();
            }

            return Json(carreras, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Nuevo()
        {
            //modelo para regresar
            CarreraModeloNuevo modelo = new CarreraModeloNuevo();
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
            modelo.InsertarCarrera = new InsertarCarrera();

            return View(modelo);
        }

        [HttpPost]
        public ActionResult Nuevo(CarreraModeloNuevo modelo)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {

                        {
                            var newCarrera = new carrera();
                            newCarrera.nombre = modelo.InsertarCarrera.Nombre;
                            newCarrera.idPeriodo = modelo.InsertarCarrera.IdPeriodo;

                            db.carrera.Add(newCarrera);
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Carrera ingresada";
                            ViewBag.exito = TempData["exito"];

                            return RedirectToAction("/");
                        }
                    }
                }
                else
                {
                    return View(modelo);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public ActionResult Editar(int id)
        {
            CarreraModeloNuevo modelo = new CarreraModeloNuevo();

            InsertarCarrera carrera = new InsertarCarrera();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var carreraTemp = db.carrera.Find(id);
                carrera.Id = carreraTemp.id;
                carrera.Nombre = carreraTemp.nombre;
                carrera.IdPeriodo = (int)carreraTemp.idPeriodo;

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

            modelo.InsertarCarrera = carrera;
            modelo.Periodos = lst;

            return View(modelo);
        }

        [HttpPost]
        public ActionResult Editar(CarreraModeloNuevo modelo)
        {

            ListPeriodos periodo = new ListPeriodos();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        var newCarrera = db.carrera.Find(modelo.InsertarCarrera.Id);
                        newCarrera.nombre = modelo.InsertarCarrera.Nombre;
                        newCarrera.idPeriodo = modelo.InsertarCarrera.IdPeriodo;

                        db.Entry(newCarrera).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //enviar exito
                        TempData["exito"] = "Carrera actualizada";
                        ViewBag.exito = TempData["exito"];

                        return RedirectToAction("/");

                    }


                }

                return View(modelo);
            }
            catch
            {

                TempData["error"] = "Algo salio mal intente nuevamente!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("/");
            }

        }

        [HttpGet]
        public JsonResult Eliminar(int idCarrera)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    carrera carreraEliminar = db.carrera.Find(idCarrera);
                    db.carrera.Remove(carreraEliminar);
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
