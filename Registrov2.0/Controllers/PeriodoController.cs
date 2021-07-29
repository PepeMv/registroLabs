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
    public class PeriodoController : Controller
    {
        // GET: Periodo
        public ActionResult Index()
        {
            List<ListPeriodos> lst = new List<ListPeriodos>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.periodo
                       select new ListPeriodos
                       {
                           Id = d.id,
                           Nombre = d.nombre,
                           FechaInicio = (DateTime)d.fechaInicio,
                           FechaFin = (DateTime)d.fechaFin
                       }).ToList();
            }

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

            return View(lst);
        }

        public ActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(InsertarPeriodo modelo)
        {

            if ((modelo.FechaFin <= modelo.FechaInicio))
            {
                TempData["error"] = "El periodo ingresado es incorrecto!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("/");
            }

            ListPeriodos periodo = new ListPeriodos();
            try
            {
                if (ModelState.IsValid)
                {
                    using ( registroLabsEntities db = new registroLabsEntities() )
                    {
                        periodo = (from p in db.periodo
                                   where (p.fechaInicio == modelo.FechaInicio && p.fechaFin == modelo.FechaFin) ||
                                            (modelo.FechaInicio > p.fechaInicio && modelo.FechaInicio < p.fechaFin) ||
                                            (modelo.FechaFin > p.fechaInicio && modelo.FechaFin < p.fechaFin) ||
                                            (modelo.FechaInicio == p.fechaInicio) ||
                                            (modelo.FechaFin == p.fechaFin) ||
                                            (p.fechaInicio > modelo.FechaInicio && p.fechaInicio < modelo.FechaFin) ||
                                            (p.fechaFin > modelo.FechaInicio & p.fechaFin < modelo.FechaFin)
                                   select new ListPeriodos {
                                       Id = p.id,
                                       Nombre = p.nombre,
                                       FechaInicio = (DateTime)p.fechaInicio,
                                       FechaFin = (DateTime)p.fechaFin
                                   }).FirstOrDefault();

                        if (periodo == null)
                        {
                            var newPeriodo = new periodo();
                            newPeriodo.nombre = modelo.Nombre;
                            newPeriodo.fechaInicio = modelo.FechaInicio;
                            newPeriodo.fechaFin = modelo.FechaFin;

                            db.periodo.Add(newPeriodo);
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Periodo ingresado";
                            ViewBag.exito = TempData["exito"];

                            return RedirectToAction("/");
                        }
                        else
                        {
                            //enviar error
                            TempData["error"] = "Ese periodo ya esta en uso!";
                            ViewBag.error = TempData["error"];

                            return RedirectToAction("/");
                        }

                        

                    }

                    
                }

                return View(modelo);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public ActionResult Editar(int id)
        {
            InsertarPeriodo periodo = new InsertarPeriodo();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var periodoTemp = db.periodo.Find(id);
                periodo.Id = periodoTemp.id;
                periodo.Nombre = periodoTemp.nombre;
                periodo.FechaInicio = (DateTime)periodoTemp.fechaInicio;
                periodo.FechaFin = (DateTime)periodoTemp.fechaFin;
            }
                return View(periodo);
        }

        [HttpPost]
        public ActionResult Editar(InsertarPeriodo modelo)
        {

            if ((modelo.FechaFin <= modelo.FechaInicio))
            {
                TempData["error"] = "El periodo ingresado es incorrecto!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("/");
            }

            ListPeriodos periodo = new ListPeriodos();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                            var newPeriodo = db.periodo.Find(modelo.Id);
                            newPeriodo.nombre = modelo.Nombre;
                            newPeriodo.fechaInicio = modelo.FechaInicio;
                            newPeriodo.fechaFin = modelo.FechaFin;

                            db.Entry(newPeriodo).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Periodo actualizado";
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
        public JsonResult Eliminar(int idPeriodo)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    periodo periodoEliminar = db.periodo.Find(idPeriodo);
                    db.periodo.Remove(periodoEliminar);
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
