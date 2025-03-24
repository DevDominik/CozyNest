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
    /// <summary>
    /// Egy attribútum, amely biztosítja, hogy a hozzáféréshez szükséges legyen token, amit a rendszer RefreshTokenként kezel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireRefreshTokenAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            HttpContext httpContext = context.HttpContext;

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out StringValues authHeader) ||
                string.IsNullOrWhiteSpace(authHeader))
            {
                context.Result = new ObjectResult(new { message = "Hiányzó vagy hibás Authorization fejléc." }) { StatusCode = 401 };
                return;
            }

            string token = authHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new ObjectResult(new { message = "Hiányzó vagy hibás Authorization fejléc." }) { StatusCode = 401 };
                return;
            }

            User? user = await UserHandler.GetUserByRefreshToken(token);
            if (user == null)
            {
                context.Result = new ObjectResult(new { message = "Érvénytelen refresh token." }) { StatusCode = 403 };
                return;
            }
            if (user.Closed)
            {
                context.Result = new ObjectResult(new { message = "Felhasználói fiók zárolt." }) { StatusCode = 403 };
            }
            Role? role = await UserHandler.GetRoleById(user.RoleId);
            if (role == null)
            {
                context.Result = new ObjectResult(new { message = "Érvénytelen szerepkör." }) { StatusCode = 403 };
                return;
            }
            context.HttpContext.Items["Token"] = token;
            context.HttpContext.Items["User"] = user;
            context.HttpContext.Items["Role"] = role;
        }
    }
}
