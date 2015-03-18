using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IntelliTaxi_Entities;
using Microsoft.AspNet.Identity;
using IntelliTaxi_Entities.Attributes;
using System.Threading.Tasks;

namespace IntelliTaxi.Controllers
{
    [HasUserData]
    [AuthorizeSimple(Optional = true)]
    public class OrdersController : Controller
    {
        private Entities db = new Entities();

        [Route("Comenzi")]
        public ActionResult Index()
        {
            int cook = Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value);
            var q = from o in db.Orders
                    where o.User_ID == cook
                    select o;
            if (HttpContext.Request.IsAuthenticated == true)
            {
                string uid = HttpContext.User.Identity.GetUserId();
                IQueryable<int> uq = from u in db.Users
                                     where u.UserID == uid
                                     select u.ID;
                List<int> ua = uq.ToList();
                q = from o in db.Orders
                        where ((o.User_ID == cook) || (ua.Contains((int)o.User_ID)))
                        select o;
            }
            return View(q.ToList());
        }

        // GET: Orders/Details/5
        [Route("Comanda/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            if (HttpContext.Request.IsAuthenticated == true)
            {
                string uid = HttpContext.User.Identity.GetUserId();
                IQueryable<int> uq = from u in db.Users
                                     where u.UserID == uid
                                     select u.ID;
                List<int> ua = uq.ToList();
                if (orders.User_ID != Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value) || !ua.Contains((int)orders.User_ID))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                if (orders.User_ID != Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
            if (orders.Taken == true && orders.Review_ID != null)
            {
                Reviews reviews = db.Reviews.Find(orders.Review_ID);
                ViewBag.review = reviews;
            }
            if (orders.Taken == true)
            {
                Taxies taxi = db.Taxies.Find(orders.TaxiTaken_ID);
                Users tuser = db.Users.Find(taxi.User_ID);
                Devices tdev = db.Devices.Find(tuser.Device_ID);
                ViewBag.TaxiLocation = new Location { Lat = tdev.Location_Lat, Long = tdev.Location_Long, Name = tdev.Location_Name, SupportsMap = tdev.Location_SupportsMap };
            }
            return View(orders);
        }

        // GET: Orders/Create
        [Route("Comanda/Noua")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Comanda/Noua")]
        public ActionResult Create([Bind(Include = "From_Lat,From_Long,From_Name,From_SupportsMap,Destination_Lat,Destination_Long,Destination_Name,Destination_SupportsMap")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders.User_ID = Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value);
                orders.DateReg = DateTime.Now;
                orders.Taken = false;
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            string uid = HttpContext.User.Identity.GetUserId();
            IQueryable<int> uq = from u in db.Users
                                 where u.UserID == uid
                                 select u.ID;
            List<int> ua = uq.ToList();
            if (orders.User_ID != Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value) || !ua.Contains((int)orders.User_ID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            else if (orders.Taken == true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db.Orders.Remove(orders);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [AjaxRequest]
        [HttpPost]
        public async Task<JsonResult> GetOrderAsync(int id)
        {
            Orders orders = await db.Orders.FindAsync(id);
            
            TimeSpan lon = DateTime.Now - (DateTime)orders.DateReg;
            dynamic a = new { TimeSp = "acum " + ((lon.Hours > 0) ? lon.Hours + " ore " : "") + ((lon.Minutes > 0) ? lon.Minutes + " minute " : "") + ((lon.Seconds > 0) ? lon.Seconds + " secunde" : ""), Taken = orders.Taken };
            return Json(a);
        }

        [AjaxRequest]
        [HttpPost]
        public async Task<JsonResult> GetTaxiLocationAsync(int id)
        {
            Taxies taxi = await db.Taxies.FindAsync(id);
            Users user = await db.Users.FindAsync(taxi.User_ID);
            Devices dev = await db.Devices.FindAsync(user.Device_ID);
            Location Location = new Location { Lat = dev.Location_Lat, Long = dev.Location_Long, Name = dev.Location_Name, SupportsMap = dev.Location_SupportsMap };
            return Json(Location);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
