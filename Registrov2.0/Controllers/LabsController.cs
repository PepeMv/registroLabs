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
    public class LabsController : Controller
    {

        // GET: Docente
        public ActionResult Index()
        {
            List<LaboratorioSimple> lst = new List<LaboratorioSimple>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.laboratorio
                       select new LaboratorioSimple
                       {
                           Id = d.id,
                           Nombre = d.nombre,
                           Capacidad = d.capacidad.HasValue ? d.capacidad.Value : 0,
                           Subred = d.subred.HasValue ? d.subred.Value : 0
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
        public ActionResult Nuevo(LaboratorioInsertEdit modelo)
        {

            LaboratorioSimple laboratorio = new LaboratorioSimple();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        laboratorio = (from p in db.laboratorio
                                       where p.nombre == modelo.Nombre
                                       select new LaboratorioSimple
                                       {
                                           Nombre = p.nombre,
                                           Capacidad = p.capacidad.HasValue ? p.capacidad.Value : 0,
                                           Subred = p.subred.HasValue ? p.subred.Value : 0
                                       }).FirstOrDefault();

                        if (laboratorio == null)
                        {
                            var newlaboratorio = new laboratorio();
                            newlaboratorio.nombre = modelo.Nombre;
                            newlaboratorio.capacidad = modelo.Capacidad;
                            newlaboratorio.subred = modelo.Subred;

                            db.laboratorio.Add(newlaboratorio);
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Laboratorio ingresado";
                            ViewBag.error = TempData["exito"];

                            return RedirectToAction("/");
                        }
                        else
                        {
                            //enviar error
                            TempData["error"] = "Ese laboratorio ya existe!";
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
        public ActionResult Editar(int Id)
        {
            LaboratorioInsertEdit Laboratorio = new LaboratorioInsertEdit();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var LaboratorioTemp = db.laboratorio.Find(Id);
                Laboratorio.Id = LaboratorioTemp.id;
                Laboratorio.Nombre = LaboratorioTemp.nombre;
                Laboratorio.Capacidad = LaboratorioTemp.capacidad.HasValue ? LaboratorioTemp.capacidad.Value : 0;
                Laboratorio.Subred = LaboratorioTemp.subred.HasValue ? LaboratorioTemp.subred.Value : 0;


            }
            return View(Laboratorio);
        }


        [HttpPost]
        public ActionResult Editar(LaboratorioInsertEdit modelo)
        {

            LaboratorioSimple laboratorio = new LaboratorioSimple();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        var newLaboratorio = db.laboratorio.Find(modelo.Id);


                        //  var newDocente = db.docente.Find(modelo.Id);
                        newLaboratorio.id = modelo.Id;
                        newLaboratorio.nombre = modelo.Nombre;
                        newLaboratorio.capacidad = modelo.Capacidad;
                        newLaboratorio.subred = modelo.Subred;


                        db.Entry(newLaboratorio).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //enviar exito
                        TempData["exito"] = "Laboratorio actualizado";
                        ViewBag.error = TempData["exito"];

                        return RedirectToAction("/");

                    }


                }

                return View(modelo);
            }
            catch (Exception )
            {

                TempData["error"] = "Algo salio mal intente nuevamente!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("/");
            }

        }

        [HttpGet]
        public JsonResult Eliminar(int idLaboratorio)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    laboratorio laboratorioEliminar = db.laboratorio.Find(idLaboratorio);
                    db.laboratorio.Remove(laboratorioEliminar);
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
