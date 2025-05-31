using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalaxyCinema.Attribute
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var sessionRole = httpContext.Session["roleName"] as string;
            return _roles.Contains(sessionRole);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Login/Login");
        }
    }
}
