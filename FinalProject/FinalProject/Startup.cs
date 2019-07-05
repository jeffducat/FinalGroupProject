using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinalProject.Startup))]
namespace FinalProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
