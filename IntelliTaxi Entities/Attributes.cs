using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel;

namespace IntelliTaxi_Entities.Attributes
{
    public class AjaxRequestAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }

    public class HasUserDataAttribute : ActionFilterAttribute
    {
        static Library.Library lib = new Library.Library();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (lib.HasUserData())
            {
                lib.UpdateUserData();
            }
            else
            {
                lib.CreateUserData();
            }
        }
    }

    public class AuthorizeSimpleAttribute : AuthorizeAttribute
    {
        static Library.Library lib = new Library.Library();
        [DefaultValue(false)]
        public bool Optional { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated && Optional == false)
            {
                if (!(filterContext.RouteData.Values["controller"] == "Account" && filterContext.RouteData.Values["action"] == "Login"))
                {
                    filterContext.HttpContext.Response.RedirectToRoute(new { controller = "Account", action = "Login" });
                }
            }
            else if (filterContext.HttpContext.Request.IsAuthenticated && lib.GetUserRole(filterContext.HttpContext.User.Identity.GetUserId()) != "")
            {
                filterContext.HttpContext.Response.RedirectToRoute(new { controller = "Account", action = "LogOffInternal" });
            }
        }
    }

    public class AuthorizeManagerAttribute : AuthorizeAttribute
    {
        static Library.Library lib = new Library.Library();

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (!(filterContext.RouteData.Values["controller"] == "Account" && filterContext.RouteData.Values["action"] == "Login"))
                {
                    filterContext.HttpContext.Response.RedirectToRoute(new { controller = "Account", action = "Login" });
                }
            }
            else if (filterContext.HttpContext.Request.IsAuthenticated && lib.GetUserRole(filterContext.HttpContext.User.Identity.GetUserId()) == "")
            {
                filterContext.HttpContext.Response.RedirectToRoute(new { controller = "Account", action = "LogOffInternal" });
            }
        }
    }

    public class UnauthorizedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.HttpContext.Response.RedirectToRoute(new { controller = "Home", action = "Index" });
            }
        }
    }
}
