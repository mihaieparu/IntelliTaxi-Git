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

namespace IntelliTaxi_Manager.Controllers
{
    [AuthorizeManager]
    [HasUserData]
    public class TaxiesController : Controller
    {
        private Entities db = new Entities();

        // GET: Taxies
        [Route("Taxiuri")]
        public ActionResult Index()
        {
            return View(db.Taxies.ToList());
        }

        // GET: Taxies/Details/5
        [Route("Taxi/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Taxies taxies = db.Taxies.Find(id);
            if (taxies == null)
            {
                return HttpNotFound();
            }
            return View(taxies);
        }

        // GET: Taxies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Taxies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RegNumber,Company_ID,HasOrder,CurrOrder_ID,User_ID")] Taxies taxies)
        {
            if (ModelState.IsValid)
            {
                db.Taxies.Add(taxies);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(taxies);
        }

        // GET: Taxies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Taxies taxies = db.Taxies.Find(id);
            if (taxies == null)
            {
                return HttpNotFound();
            }
            return View(taxies);
        }

        // POST: Taxies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RegNumber,Company_ID,HasOrder,CurrOrder_ID,User_ID")] Taxies taxies)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taxies).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taxies);
        }

        // GET: Taxies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Taxies taxies = db.Taxies.Find(id);
            if (taxies == null)
            {
                return HttpNotFound();
            }
            return View(taxies);
        }

        // POST: Taxies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Taxies taxies = db.Taxies.Find(id);
            db.Taxies.Remove(taxies);
            db.SaveChanges();
            return RedirectToAction("Index");
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
