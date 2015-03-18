using IntelliTaxi_Entities;
using IntelliTaxi_Entities.Attributes;
using IntelliTaxi_Entities.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace IntelliTaxi.Controllers
{
    [HasUserData]
    [AuthorizeSimple(Optional = true)]
    public class HomeController : Controller
    {
        static Entities db = new Entities();
        static Library lib = new Library();

        public ActionResult Index()
        {
            var getcompanies = from c in db.Companies
                            select new { ID = c.ID, Name = c.Name, Price = c.Price, City = c.City };
            int cook = Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value);
            var q = from o in db.Orders
                    where (o.User_ID == cook && o.Taken == false)
                    select o;
            if (HttpContext.Request.IsAuthenticated == true)
            {
                string uid = HttpContext.User.Identity.GetUserId();
                IQueryable<int> uq = from u in db.Users
                                     where u.UserID == uid
                                     select u.ID;
                List<int> ua = uq.ToList();
                q = from o in db.Orders
                    where (((o.User_ID == cook) || (ua.Contains((int)o.User_ID))) && o.Taken == false)
                    select o;
            }
            ViewBag.orders = (q.Count() > 0) ? q : null;
            ViewBag.companies = (getcompanies.Count() > 0) ? getcompanies : null;
            return View();
        }

        [Route("Despre")]
        public ActionResult About()
        {
            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            return View();
        }
    }
}