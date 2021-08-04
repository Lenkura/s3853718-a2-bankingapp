using AdminPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AdminPortal.Authorise
{
    public class SecureContentAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var customerID = context.HttpContext.Session.GetString("login");
            if (customerID == null)
                context.Result = new RedirectToActionResult("Index", "Login", null);
        }
    }
}
