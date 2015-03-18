using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IntelliTaxi_Entities;
using IntelliTaxi_Entities.Attributes;
using System.Threading.Tasks;

namespace IntelliTaxi.Controllers
{
    [HasUserData]
    [AuthorizeSimple(Optional = true)]
    public class ReviewsController : Controller
    {
        private Entities db = new Entities();

        // GET: Reviews/Create
        [Route("Review/Nou")]
        public ActionResult Create(int? oid)
        {
            if (oid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.oid = oid;
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Review/Nou")]
        public ActionResult Create([Bind(Include = "For_ID,Stars,Content")] Reviews reviews)
        {
            if (ModelState.IsValid)
            {
                reviews.From_ID = Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value);
                reviews.SendDate = DateTime.Now;
                Orders order = db.Orders.Find(reviews.For_ID);
                order.Review_ID = db.Reviews.Last().ID;
                db.Reviews.Add(reviews);
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Orders");
            }

            return View(reviews);
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
