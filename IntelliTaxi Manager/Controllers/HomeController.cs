using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntelliTaxi_Entities;
using IntelliTaxi_Entities.Attributes;
using IntelliTaxi_Entities.Library;
using IntelliTaxi_Entities.DataContexts;

namespace IntelliTaxi_Manager.Controllers
{
    [HasUserData]
    [AuthorizeManager]
    public class HomeController : Controller
    {
        static Library lib = new Library();
        static Entities db = new Entities();
        static ApplicationDbContext udb = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.Role = lib.GetUserRole();
            var getontaxiesorders = from t in db.Taxies
                                    join u in db.Users on t.ID equals u.ID
                                    join d in db.Devices on u.Device_ID equals d.ID
                                    join o in db.Orders on t.CurrOrder_ID equals o.ID
                                    where d.On == true
                                    select new { TaxiID = t.ID, TaxiRegNumber = t.RegNumber, TaxiHasOrder = t.HasOrder, UserID = u.ID, UserName = u.Name, DeviceId = d.ID, DeviceIP = d.IP, DeviceLastUse = d.LastUse, DeviceOn = d.On, DeviceType = d.Type, DeviceLocLat = d.Location_Lat, DviceLocLong = d.Location_Long, DeviceLocName = d.Location_Name, DeviceLocSM = d.Location_SupportsMap, OrderID = o.ID, OrderDateReg = o.DateReg, OrderDateTaken = o.DateTaken, OrderDateLeft = o.DateLeft, OrderTaken = o.Taken, OrderFromLat = o.From_Lat, OrderFromLong = o.From_Long, OrderFromName = o.From_Name, OrderFromSM = o.From_SupportsMap, OrderDestinationLat = o.Destination_Lat, OrderDestinationLong = o.Destination_Long, OrderDestinationName = o.Destination_Name, OrderDestinationSM = o.Destination_SupportsMap };
            var getallcompanytaxies = from t in db.Taxies
                                      select new { ID = t.ID };
            if (ViewBag.Role == "Company")
            {
                var companyid = lib.GetCompanyID();
                getontaxiesorders = from t in db.Taxies
                                    join u in db.Users on t.ID equals u.ID
                                    join d in db.Devices on u.Device_ID equals d.ID
                                    join o in db.Orders on t.CurrOrder_ID equals o.ID
                                    where (d.On == true && t.Company_ID == companyid)
                                    select new { TaxiID = t.ID, TaxiRegNumber = t.RegNumber, TaxiHasOrder = t.HasOrder, UserID = u.ID, UserName = u.Name, DeviceId = d.ID, DeviceIP = d.IP, DeviceLastUse = d.LastUse, DeviceOn = d.On, DeviceType = d.Type, DeviceLocLat = d.Location_Lat, DviceLocLong = d.Location_Long, DeviceLocName = d.Location_Name, DeviceLocSM = d.Location_SupportsMap, OrderID = o.ID, OrderDateReg = o.DateReg, OrderDateTaken = o.DateTaken, OrderDateLeft = o.DateLeft, OrderTaken = o.Taken, OrderFromLat = o.From_Lat, OrderFromLong = o.From_Long, OrderFromName = o.From_Name, OrderFromSM = o.From_SupportsMap, OrderDestinationLat = o.Destination_Lat, OrderDestinationLong = o.Destination_Long, OrderDestinationName = o.Destination_Name, OrderDestinationSM = o.Destination_SupportsMap };
                getallcompanytaxies = from t in db.Taxies
                                      where t.Company_ID == lib.GetCompanyID("loggedin")
                                      select new { ID = t.ID };
            }
            var getuntookorders = from o in db.Orders
                                  join u in db.Users on o.User_ID equals u.ID
                                  join d in db.Devices on u.Device_ID equals d.ID
                                  where o.Taken == false
                                  select new { OrderID = o.ID, OrderDateReg = o.DateReg, OrderDateTaken = o.DateTaken, OrderDateLeft = o.DateLeft, OrderTaken = o.Taken, OrderFromLat = o.From_Lat, OrderFromLong = o.From_Long, OrderFromName = o.From_Name, OrderFromSM = o.From_SupportsMap, OrderDestinationLat = o.Destination_Lat, OrderDestinationLong = o.Destination_Long, OrderDestinationName = o.Destination_Name, OrderDestinationSM = o.Destination_SupportsMap, UserID = u.ID, UserName = u.Name, DeviceId = d.ID, DeviceIP = d.IP, DeviceLastUse = d.LastUse, DeviceOn = d.On, DeviceType = d.Type, DeviceLocLat = d.Location_Lat, DviceLocLong = d.Location_Long, DeviceLocName = d.Location_Name, DeviceLocSM = d.Location_SupportsMap };
            var getcompanies = from c in db.Companies
                               join u in db.Users on c.Admin_ID equals u.ID
                               join d in db.Devices on u.Device_ID equals d.ID
                               select new { CompanyID = c.ID, CompanyName = c.Name, CompanyCity = c.City, CompanyPrice = c.Price, UserID = u.ID, UserName = u.Name, DeviceID = d.ID, DeviceIP = d.IP, DeviceLastUse = d.LastUse, DeviceOn = d.On, DeviceType = d.Type };
            var getusers = from u in db.Users
                           join d in db.Devices on u.Device_ID equals d.ID
                           where d.On == true
                           select new { UserID = u.ID, UserName = u.Name, DeviceID = d.ID, DeviceIP = d.IP, DeviceLastUse = d.LastUse, DeviceOn = d.On, DeviceType = d.Type, DeviceLocLat = d.Location_Lat, DviceLocLong = d.Location_Long, DeviceLocName = d.Location_Name, DeviceLocSM = d.Location_SupportsMap };
            switch (lib.GetUserRole()) {
                case "Admin":
                    ViewBag.companies = getcompanies.Count();
                    ViewBag.users = new { On = getusers.Count(), All = db.Users.Count() };
                    ViewBag.taxies = new { On = getontaxiesorders.Count(), All = db.Taxies.Count() };
                    break;
                case "Company":
                    ViewBag.taxies = new { On = getontaxiesorders.Count(), All = getallcompanytaxies.Count() };
                    ViewBag.untookorders = getuntookorders.Count();
                    break;
                case "Taxi":
                    ViewBag.untookorders = (getuntookorders.Count() > 0) ? getuntookorders.ToList() : null;
                    break;
            }
            return View();
        }

        [Route("Despre")]
        public ActionResult About()
        {
            ViewBag.Role = lib.GetUserRole();
            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            return View();
        }
    }
}