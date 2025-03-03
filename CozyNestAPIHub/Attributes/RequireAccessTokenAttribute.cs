using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using CozyNestAPIHub.Handlers;
using CozyNestAPIHub.Models;

namespace CozyNestAPIHub.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireAccessTokenAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            HttpContext httpContext = context.HttpContext;

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out StringValues authHeader) ||
                string.IsNullOrWhiteSpace(authHeader))
            {
                context.Result = new ObjectResult(new { message = "Missing or invalid Authorization header." }) { StatusCode = 401 };
                return;
            }

            string token = authHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new ObjectResult(new { message = "Missing or invalid Authorization header." }) { StatusCode = 401 };
                return;
            }

            User? user = await UserHandler.GetUserByAccessToken(token);
            if (user == null)
            {
                context.Result = new ObjectResult(new { message = "Invalid access token." }) { StatusCode = 403 };
                return;
            }

            context.HttpContext.Items["Token"] = token;
            context.HttpContext.Items["User"] = user;
        }
    }
}
