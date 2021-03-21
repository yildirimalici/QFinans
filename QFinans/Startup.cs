using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QFinans.Startup))]
namespace QFinans
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
