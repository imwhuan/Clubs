using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClubApp.Startup))]
namespace ClubApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}