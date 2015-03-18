using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using IntelliTaxi_Entities;
using Microsoft.AspNet.Identity;
using IntelliTaxi_Entities.Attributes;

namespace IntelliTaxi_Entities.Controllers
{
    public class AJAXController : Controller
    {
        static Entities db = new Entities();
        static Library.Library lib = new Library.Library();

        [AjaxRequest]
        [HttpPost]
        public async Task<JsonResult> DisconnectAsync()
        {
            if (lib.HasUserData())
            {
                Devices device = await db.Devices.FindAsync(Convert.ToInt32(HttpContext.Request.Cookies["Device_ID"].Value));
                device.LastUse = DateTime.Now;
                device.IP = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                device.On = false;
                db.Entry(device).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json("ready");
            }
            else
            {
                return Json("error");
            }
        }

        [AjaxRequest]
        [HttpPost]
        public async Task<JsonResult> UpdateUserDataAsync(string Lat = "0", string Long = "0", string Name = "", string SupportsMap = "false", string DType = "Desktop")
        {
            if (lib.HasUserData())
            {
                Devices device = await db.Devices.FindAsync(Convert.ToInt32(HttpContext.Request.Cookies["Device_ID"].Value));
                Users user = await db.Users.FindAsync(Convert.ToInt32(HttpContext.Request.Cookies["User_ID"].Value));
                DeviceType dtype;
                Enum.TryParse(DType, out dtype);
                device.Type = (int)dtype;
                device.LastUse = DateTime.Now;
                device.IP = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                device.On = true;
                device.Location_Lat = Convert.ToDouble(Lat);
                device.Location_Long = Convert.ToDouble(Long);
                device.Location_Name = Name;
                device.Location_SupportsMap = Convert.ToBoolean(SupportsMap);
                db.Entry(device).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (HttpContext.Request.IsAuthenticated)
                {
                    user.UserID = HttpContext.User.Identity.GetUserId();
                    user.Name = HttpContext.User.Identity.GetUserName();
                    lib.CreateCookie("UserID", user.UserID, DateTime.Now.AddYears(1));
                }
                else
                {
                    user.UserID = null;
                    user.Name = null;
                    lib.DeleteCookie("UserID");
                }
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                lib.CreateCookie("User_ID", user.ID.ToString(), DateTime.Now.AddYears(1));
                lib.CreateCookie("Device_ID", device.ID.ToString(), DateTime.Now.AddYears(1));
                return Json("ready");
            }
            else
            {
                return await CreateUserDataAsync(Lat, Long, Name, SupportsMap);
            }
        }

        [AjaxRequest]
        [HttpPost]
        public async Task<JsonResult> CreateUserDataAsync(string Lat = "0", string Long = "0", string Name = "", string SupportsMap = "false", string DType = "Desktop")
        {
            if (!lib.HasUserData())
            {
                Users user = new Users();
                Devices device = new Devices();
                IQueryable<int> a = db.Devices.OrderByDescending(r => r.ID).Take(1).Select(r => r.ID);
                if (a.Count() > 0)
                {
                    device.ID = (int)a.ToList()[0] + 1;
                }
                IQueryable<int> b = db.Users.OrderByDescending(r => r.ID).Take(1).Select(r => r.ID);
                if (b.Count() > 0)
                {
                    user.ID = (int)b.ToList()[0] + 1;
                }
                device.FirstUse = DateTime.Now;
                device.LastUse = DateTime.Now;
                DeviceType dtype;
                Enum.TryParse(DType, out dtype);
                device.Type = (int)dtype;
                device.IP = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                device.On = true;
                device.Location_Lat = Convert.ToDouble(Lat);
                device.Location_Long = Convert.ToDouble(Long);
                device.Location_Name = Name;
                device.Location_SupportsMap = Convert.ToBoolean(SupportsMap);
                if (HttpContext.Request.IsAuthenticated)
                {
                    user.UserID = HttpContext.User.Identity.GetUserId();
                    user.Name = HttpContext.User.Identity.GetUserName();
                }
                db.Users.Add(user);
                db.Devices.Add(device);
                await db.SaveChangesAsync();
                user.Device_ID = device.ID;
                device.User_ID = user.ID;
                db.Entry(device).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                lib.CreateCookie("User_ID", user.ID.ToString(), DateTime.Now.AddYears(1));
                lib.CreateCookie("UserID", user.UserID, DateTime.Now.AddYears(1));
                lib.CreateCookie("Device_ID", device.ID.ToString(), DateTime.Now.AddYears(1));
                return Json("ready");
            }
            else
            {
                return Json("error");
            }
        }
    }
}