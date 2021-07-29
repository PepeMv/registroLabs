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
    public class EstudianteController : Controller
    {
        // GET: Docente
        public ActionResult Index()
        {
            List<EstudianteSimple> lst = new List<EstudianteSimple>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.estudiante
                       select new EstudianteSimple
                       {
                           Id = d.id,
                           Cedula = d.cedula,
                           Nombre = d.nombre,
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


        // hasta aqui 
        public ActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(EstudianteInsertEdit modelo)
        {

            EstudianteSimple estudiante = new EstudianteSimple();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        estudiante = (from p in db.estudiante
                                      where p.cedula == modelo.Cedula
                                      select new EstudianteSimple
                                      {
                                          Cedula = p.cedula,
                                          Nombre = p.nombre,

                                      }).FirstOrDefault();

                        if (estudiante == null)
                        {
                            var newdestudiante = new estudiante();
                            newdestudiante.cedula = modelo.Cedula;
                            newdestudiante.nombre = modelo.Nombre;

                            db.estudiante.Add(newdestudiante);
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Estudiante ingresado";
                            ViewBag.error = TempData["exito"];

                            return RedirectToAction("/");
                        }
                        else
                        {
                            //enviar error
                            TempData["error"] = "Ese estudiante ya existe!";
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
            EstudianteInsertEdit estudiante = new EstudianteInsertEdit();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var estudianteTemp = db.estudiante.Find(Id);
                estudiante.Id = estudianteTemp.id;
                estudiante.Cedula = estudianteTemp.cedula;
                estudiante.Nombre = estudianteTemp.nombre;

            }
            return View(estudiante);
        }


        [HttpPost]
        public ActionResult Editar(EstudianteInsertEdit modelo)
        {

            EstudianteSimple estudiante = new EstudianteSimple();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        var newEstudiante = db.estudiante.Find(modelo.Id);


                        //  var newDocente = db.docente.Find(modelo.Id);
                        newEstudiante.id = modelo.Id;
                        newEstudiante.cedula = modelo.Cedula;
                        newEstudiante.nombre = modelo.Nombre;



                        db.Entry(newEstudiante).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //enviar exito
                        TempData["exito"] = "Estudiante actualizado";
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
        public JsonResult Eliminar(int idEstudiante)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    estudiante estudianteEliminar = db.estudiante.Find(idEstudiante);
                    db.estudiante.Remove(estudianteEliminar);
                    int v = db.SaveChanges();

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
