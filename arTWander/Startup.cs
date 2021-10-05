using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(arTWander.Startup))]
namespace arTWander
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
