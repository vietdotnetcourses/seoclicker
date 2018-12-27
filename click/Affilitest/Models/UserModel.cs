using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Affilitest.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Role { get; set; }

        public static HttpCookie GetAuthenticationCookie(UserModel model, UserCookieData userData)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                model.UserName,
                DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                true,
                JsonConvert.SerializeObject(userData), FormsAuthentication.FormsCookiePath
                );
            string encTicket = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            return cookie;

        }
    }

}