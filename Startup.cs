using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Katie_Rosario_Blog.Startup))]
namespace Katie_Rosario_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
