using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Centerflix.Startup))]
namespace Centerflix
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
