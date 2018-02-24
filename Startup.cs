using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bengo.Startup))]
namespace Bengo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
