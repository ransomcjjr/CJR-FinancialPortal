using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CJR_FinancialPortal.Models
{
    public static class RefreshAuth
    {
        public static async Task RefreshAuthentication(this HttpContextBase contex, ApplicationUser user)
        {
            contex.GetOwinContext().Authentication.SignOut();
            await contex.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
    }
}