using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CozyNestAPIHub.Handlers;
using System.Linq;
using System.Threading.Tasks;
using CozyNestAPIHub.Models;

namespace CozyNestAPIHub.Attributes
{
    /// <summary>
    /// Egy attribútum, amely biztosítja, hogy a hozzáféréshez szükséges legyen megfelelő szerepkör.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class RoleAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _roles;

        public RoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var (isValid, errorMessage, errorCode) = await CheckUserRole(context.HttpContext);
            if (!isValid)
            {
                context.Result = new ObjectResult(new { message = errorMessage }) { StatusCode = errorCode };
                return;
            }

            await next();
        }

        private async Task<(bool, string?, int)> CheckUserRole(HttpContext context)
        {
            Role role = await GetItemFromContext<Role>(context, "Role");

            if (!_roles.Contains(role.Name)) return (false, "Hozzáférés elutasítva.", 403);

            return (true, null, 200);
        }
    }
}
