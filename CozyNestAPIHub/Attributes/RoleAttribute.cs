using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CozyNestAPIHub.Handlers;
using System.Linq;
using System.Threading.Tasks;
using CozyNestAPIHub.Models;

namespace CozyNestAPIHub.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RoleAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _roles;

        public RoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var (isValid, errorMessage, errorCode) = await CheckUserRole(await GetItemFromContext<User>(context.HttpContext, "User"));
            if (!isValid)
            {
                context.Result = new ObjectResult(new { message = errorMessage }) { StatusCode = errorCode };
                return;
            }

            await next();
        }

        private async Task<(bool, string?, int)> CheckUserRole(User user)
        {
            var role = UserHandler.GetRoleById(user.RoleId);
            if (role == null) return (false, "Role not found.", 404);

            if (!_roles.Contains(role.Name)) return (false, "Access denied.", 403);

            return (true, null, 200);
        }
    }
}
