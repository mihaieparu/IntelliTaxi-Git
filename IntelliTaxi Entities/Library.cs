using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using IntelliTaxi_Entities.DataContexts;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IntelliTaxi_Entities.Library
{
    public class Library
    {
        static Entities db = new Entities();
        static ApplicationDbContext appdb = new ApplicationDbContext();

        public bool HasUserData()
        {
            bool hasUserData = false;
            if (HttpContext.Current.Request.Cookies["User_ID"] != null && HttpContext.Current.Request.Cookies["User_ID"].Value != "" && HttpContext.Current.Request.Cookies["Device_ID"] != null && HttpContext.Current.Request.Cookies["Device_ID"].Value != "")
            {
                var devid = Convert.ToInt32(HttpContext.Current.Request.Cookies["Device_ID"].Value);
                var usid = Convert.ToInt32(HttpContext.Current.Request.Cookies["User_ID"].Value);
                var devq = from d in db.Devices
                           where d.ID.Equals(devid)
                           select new { d.ID };
                var usq = from u in db.Users
                           where u.ID.Equals(usid)
                           select new { u.ID };
                if (devq.Count() > 0 && usq.Count() > 0)
                {
                    hasUserData = true;
                }
                else
                {
                    DeleteCookie("User_ID");
                    DeleteCookie("Device_ID");
                    hasUserData = false;
                }
            }
            else
            {
                DeleteCookie("User_ID");
                DeleteCookie("Device_ID");
                hasUserData = false;
            }
            return hasUserData;
        }

        public void UpdateUserData()
        {
            if (HasUserData())
            {
                Devices device = db.Devices.Find(Convert.ToInt32(HttpContext.Current.Request.Cookies["Device_ID"].Value));
                Users user = db.Users.Find(Convert.ToInt32(HttpContext.Current.Request.Cookies["User_ID"].Value));
                device.LastUse = DateTime.Now;
                device.IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                device.On = true;
                db.Entry(device).State = EntityState.Modified;
                db.SaveChanges();
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    user.UserID = HttpContext.Current.User.Identity.GetUserId();
                    user.Name = HttpContext.Current.User.Identity.GetUserName();
                    CreateCookie("UserID", user.UserID, DateTime.Now.AddYears(1));
                }
                else
                {
                    user.UserID = "";
                    user.Name = "";
                    DeleteCookie("UserID");
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                CreateCookie("User_ID", user.ID.ToString(), DateTime.Now.AddYears(1));
                CreateCookie("Device_ID", device.ID.ToString(), DateTime.Now.AddYears(1));
            }
        }

        public void CreateUserData()
        {
            if (!HasUserData())
            {
                Users user = new Users();
                Devices device = new Devices();
                IQueryable<int> a = db.Devices.OrderByDescending(r => r.ID).Take(1).Select(r => r.ID);
                IQueryable<int> b = db.Users.OrderByDescending(r => r.ID).Take(1).Select(r => r.ID);
                if (b.Count() > 0)
                {
                    user.ID = (int)b.ToList()[0] + 1;
                }
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    user.UserID = HttpContext.Current.User.Identity.GetUserId();
                    user.Name = HttpContext.Current.User.Identity.GetUserName();
                }
                db.Users.Add(user);
                db.SaveChanges();
                if (a.Count() > 0)
                {
                    device.ID = (int)a.ToList()[0] + 1;
                }
                device.FirstUse = DateTime.Now;
                device.LastUse = DateTime.Now;
                device.IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                device.Type = 1;
                device.On = true;
                db.Devices.Add(device);
                db.SaveChanges();
                device.User_ID = user.ID;
                db.Entry(device).State = EntityState.Modified;
                db.SaveChanges();
                user.Device_ID = device.ID;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                CreateCookie("User_ID", user.ID.ToString(), DateTime.Now.AddYears(1));
                CreateCookie("UserID", user.UserID, DateTime.Now.AddYears(1));
                CreateCookie("Device_ID", device.ID.ToString(), DateTime.Now.AddYears(1));
            }
        }

        #region HelperClasses
        public void CreateCookie(string name, string value, DateTime expire)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            cookie.Expires = expire;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public void DeleteCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = "";
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public string GetUserRole(string id = "loggedin")
        {
            string roleName = "";
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                if (id == "loggedin")
                {
                    id = HttpContext.Current.User.Identity.GetUserId();
                }
                ApplicationUser user = appdb.Users.Find(id);
                if (user.Roles.Count > 0 && user.Roles.FirstOrDefault().RoleId != null) {
                    IdentityRole role = appdb.Roles.Find(user.Roles.FirstOrDefault().RoleId);
                    roleName = role.Name;
                }
                else
                {
                    roleName = "";
                }
            }
            else
            {
                roleName = "";
            }
            return roleName;
        }
        public int GetCompanyID(string id = "loggedin")
        {
            int companyid = 0;
            if (id == "loggedin")
            {
                id = HttpContext.Current.User.Identity.GetUserId();
            }
            var company = from u in db.Users
                          join c in db.Companies on u.ID equals c.Admin_ID
                          where u.UserID == id
                          select new { ID = c.ID };
            companyid = (company.Count() > 0) ? company.First().ID : 0;
            return companyid;
        }
        #endregion
    }
}
