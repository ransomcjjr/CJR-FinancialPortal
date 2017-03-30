using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CJR_FinancialPortal.Startup))]
namespace CJR_FinancialPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
