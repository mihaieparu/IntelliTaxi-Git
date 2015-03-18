using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IntelliTaxi.Startup))]
namespace IntelliTaxi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
