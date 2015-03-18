namespace IntelliTaxi_Manager.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using IntelliTaxi_Entities.DataContexts;

    internal sealed class Configuration : DbMigrationsConfiguration<IntelliTaxi_Entities.DataContexts.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataContexts/IdentityMigrations";
        }

        bool AddUserAndRole(ApplicationDbContext context)
        {
            IdentityResult ir;
            var rm = new RoleManager<IdentityRole> (new RoleStore<IdentityRole>(context));
            ir = rm.Create(new IdentityRole("Admin"));
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = new ApplicationUser()
            {
                UserName = "mihaieparu",
                PhoneNumber = "0723002420",
                City = "Mizil",
                Email = "mihaieparu@yahoo.com"
            };
            ir = um.Create(user, "INTELLITAXI");
            if (ir.Succeeded == false) return ir.Succeeded;
            ir = um.AddToRole(user.Id, "Admin");
            ir = rm.Create(new IdentityRole("Company"));
            ir = rm.Create(new IdentityRole("Taxi"));
            return ir.Succeeded;
        }

        bool AddUsers(ApplicationDbContext context)
        {
            IdentityResult ir;
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = new ApplicationUser()
            {
                UserName = "taxi1",
                PhoneNumber = "0723002420",
                City = "Mizil",
                Email = "mihaieparu@yahoo.com"
            };
            ir = um.Create(user, "INTELLITAXI");
            if (ir.Succeeded == false) return ir.Succeeded;
            ir = um.AddToRole(user.Id, "Taxi");
            user = new ApplicationUser()
            {
                UserName = "company1",
                PhoneNumber = "0723002420",
                City = "Mizil",
                Email = "mihaieparu@yahoo.com"
            };
            ir = um.Create(user, "INTELLITAXI");
            if (ir.Succeeded == false) return ir.Succeeded;
            ir = um.AddToRole(user.Id, "Company");
            return ir.Succeeded;
        }

        protected override void Seed(IntelliTaxi_Entities.DataContexts.ApplicationDbContext context)
        {
            AddUsers(context);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    db.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
