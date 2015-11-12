using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Ictect.IntelliDocs.Web.Startup))]
namespace Ictect.IntelliDocs.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Owin OAuth
            ConfigureAuth(app);
        }
    }
}
