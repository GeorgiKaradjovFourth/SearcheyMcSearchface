using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SearcheyMcSearchface.Startup))]
namespace SearcheyMcSearchface
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
