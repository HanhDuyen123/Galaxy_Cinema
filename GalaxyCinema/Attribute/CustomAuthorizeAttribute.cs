using System;
using System.Web;
using System.Web.Mvc;

namespace GalaxyCinema.Attribute
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _role;

        public CustomAuthorizeAttribute(string role)
        {
            _role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var sessionRole = httpContext.Session["roleName"] as string;
            return !string.IsNullOrEmpty(sessionRole) && sessionRole == _role;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Login/Login");
        }
    }
}
