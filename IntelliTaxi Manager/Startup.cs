using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IntelliTaxi_Manager.Startup))]
namespace IntelliTaxi_Manager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
