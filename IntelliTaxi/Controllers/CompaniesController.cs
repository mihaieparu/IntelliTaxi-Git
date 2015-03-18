using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IntelliTaxi_Entities;
using IntelliTaxi_Entities.Library;
using IntelliTaxi_Entities.Attributes;

namespace IntelliTaxi.Controllers
{
    [HasUserData]
    [AuthorizeSimple(Optional = true)]
    public class CompaniesController : Controller
    {
        private Entities db = new Entities();
        static Library lib = new Library();

        // GET: Companies
        [Route("Companii")]
        public ActionResult Index()
        {
            var getcompanies = from c in db.Companies
                               select new { ID = c.ID, Name = c.Name, Price = c.Price, City = c.City };
            ViewBag.companies = (getcompanies.Count() > 0) ? getcompanies : null;
            return View(db.Companies.ToList());
        }

        // GET: Companies/Details/5
        [Route("Companie/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Companies companies = db.Companies.Find(id);
            ViewBag.company = companies;
            var gettaxies = from t in db.Taxies
                            where t.Company_ID == id
                            join d in db.Devices on t.User_ID equals d.User_ID
                            select new { ID = t.ID, On = d.On };
            ViewBag.taxies = (gettaxies.Count() > 0) ? gettaxies : null;
            if (companies == null)
            {
                return HttpNotFound();
            }
            return View(companies);
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
