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
    public class DevicesController : Controller
    {
        private Entities db = new Entities();

        // GET: Devices
        [Route("Dispozitive")]
        public ActionResult Index()
        {
            return View(db.Devices.ToList());
        }

        // GET: Devices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Devices devices = db.Devices.Find(id);
            if (devices == null)
            {
                return HttpNotFound();
            }
            return View(devices);
        }

        // GET: Devices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Type,Location_Lat,Location_Long,Location_Name,Location_SupportsMap,FirstUse,LastUse,IP,On,User_ID")] Devices devices)
        {
            if (ModelState.IsValid)
            {
                db.Devices.Add(devices);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(devices);
        }

        // GET: Devices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Devices devices = db.Devices.Find(id);
            if (devices == null)
            {
                return HttpNotFound();
            }
            return View(devices);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Type,Location_Lat,Location_Long,Location_Name,Location_SupportsMap,FirstUse,LastUse,IP,On,User_ID")] Devices devices)
        {
            if (ModelState.IsValid)
            {
                db.Entry(devices).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(devices);
        }

        // GET: Devices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Devices devices = db.Devices.Find(id);
            if (devices == null)
            {
                return HttpNotFound();
            }
            return View(devices);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Devices devices = db.Devices.Find(id);
            db.Devices.Remove(devices);
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
