using Affilitest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Affilitest
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null || authCookie.Value == "")
            {
                return;
            }
            FormsAuthenticationTicket ticket;

            try
            {
                ticket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }
            var uCookie = JsonConvert.DeserializeObject<UserCookieData>(ticket.UserData);

            if (uCookie.CreatedDate == null)
            {
                FormsAuthentication.SignOut();
                HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
                return;
            }

            string[] roles = uCookie.Roles;

            if (HttpContext.Current.User != null)
            {
                HttpContext.Current.User = new GenericPrincipal(HttpContext.Current.User.Identity, roles);
            }
        }
    }
}
