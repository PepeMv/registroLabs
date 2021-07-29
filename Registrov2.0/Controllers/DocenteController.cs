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
    public class DocenteController : Controller
    {
        // GET: Docente
        public ActionResult Index()
        {
            List<DocenteVM> lst = new List<DocenteVM>();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                lst = (from d in db.docente
                       select new DocenteVM
                       {
                           Id = d.id,
                           Cedula = d.cedula,
                           Nombre = d.nombre,
                           Apellido = d.apellido
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
        public ActionResult Nuevo(DocenteInsertEdit modelo)
        {

            DocenteVM docente = new DocenteVM();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        docente = (from p in db.docente
                                   where p.cedula == modelo.Cedula
                                   select new DocenteVM
                                   {
                                       Cedula = p.cedula,
                                       Nombre = p.nombre,
                                       Apellido = p.apellido,
                                   }).FirstOrDefault();

                        if (docente == null)
                        {
                            var newdocente = new docente();
                            newdocente.cedula = modelo.Cedula;
                            newdocente.nombre = modelo.Nombre;
                            newdocente.apellido = modelo.Apellido;

                            db.docente.Add(newdocente);
                            db.SaveChanges();

                            //enviar exito
                            TempData["exito"] = "Docente ingresado";
                            ViewBag.error = TempData["exito"];

                            return RedirectToAction("/");
                        }
                        else
                        {
                            //enviar error
                            TempData["error"] = "Ese docente ya existe!";
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
            DocenteInsertEdit docente = new DocenteInsertEdit();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                var docenteTemp = db.docente.Find(Id);
                docente.Id = docenteTemp.id;
                docente.Cedula = docenteTemp.cedula;
                docente.Nombre = docenteTemp.nombre;
                docente.Apellido = docenteTemp.apellido;
            }
            return View(docente);
        }


        [HttpPost]
        public ActionResult Editar(DocenteInsertEdit modelo)
        {

            DocenteVM docente = new DocenteVM();
            try
            {
                if (ModelState.IsValid)
                {
                    using (registroLabsEntities db = new registroLabsEntities())
                    {
                        var newDocente = db.docente.Find(modelo.Id);


                        //  var newDocente = db.docente.Find(modelo.Id);
                        newDocente.id = modelo.Id;
                        newDocente.cedula = modelo.Cedula;
                        newDocente.nombre = modelo.Nombre;
                        newDocente.apellido = modelo.Apellido;


                        db.Entry(newDocente).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //enviar exito
                        TempData["exito"] = "Docente actualizado";
                        ViewBag.error = TempData["exito"];

                        return RedirectToAction("/");

                    }


                }

                return View(modelo);
            }
            catch (Exception ex)
            {

                TempData["error"] = "Algo salio mal intente nuevamente!";
                ViewBag.error = TempData["error"];

                return RedirectToAction("/");
            }

        }

        [HttpGet]
        public JsonResult Eliminar(int idDocente)
        {
            JsonRespuesta respuesta = new JsonRespuesta();

            using (registroLabsEntities db = new registroLabsEntities())
            {
                try
                {
                    docente docenteEliminar = db.docente.Find(idDocente);
                    db.docente.Remove(docenteEliminar);
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
