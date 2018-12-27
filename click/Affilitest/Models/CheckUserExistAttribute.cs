using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Affilitest.Models
{
    public class CheckUserExistAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated){
                FormsAuthentication.SignOut();
                filterContext.Result = new RedirectResult(string.Format("/Home/Login?targetUrl={0}", filterContext.HttpContext.Request.Url.AbsolutePath));
            }
                else
                using (var db = new Affilitest.Models.AffilitestdbEntities())
                {
                    var userExist = db.Users.Where(u => u.UserName == HttpContext.Current.User.Identity.Name).FirstOrDefault();
                    if (userExist == null)
                    {
                        FormsAuthentication.SignOut();
                        filterContext.Result = new RedirectResult(string.Format("/Home/Login?targetUrl={0}", filterContext.HttpContext.Request.Url.AbsolutePath));
                    }
                }

        }
    }
}