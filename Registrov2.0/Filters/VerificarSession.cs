using Registrov2._0.Controllers;
using Registrov2._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registrov2._0.Filters
{
    
    public class VerificarSession : ActionFilterAttribute
    {
        private usuario oUser;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);

                bool disabled = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                        filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
                if (disabled)
                    return;

                oUser = (usuario)HttpContext.Current.Session["user"];
                if (oUser == null)
                {
                    
                    if (filterContext.Controller is AccesoController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("/Acceso/Login");
                    }
                }
            }
            catch (Exception)
            {

                filterContext.Result = new RedirectResult("~/Acceso/Login");
            }
            
        }
    }
    
}
