using Registrov2._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Login()
        {
            Session["user"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            try
            {
                using (registroLabsEntities db = new registroLabsEntities())
                {
                    var oUser = (from d in db.usuario
                                 where d.mail == user.Trim() && d.password == password.Trim()
                                 select d).FirstOrDefault();

                    if (oUser == null)
                    {
                        ViewBag.error = "Usuario o contraseña invalido!";
                        return View();
                    }

                    Session["user"] = oUser;

                }
                    return RedirectToAction("Index", "Administracion");
            }
            catch (Exception)
            {

                return View();
            }
        }

        public void CerrarSesion()
        {
            Session["user"] = null;
        }
    }
}
