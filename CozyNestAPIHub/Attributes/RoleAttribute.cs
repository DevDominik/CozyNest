using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CozyNestAPIHub.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace CozyNestAPIHub.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAttribute(params string[] roles)
        {
            _roles = roles;
        }
        [RequireAccessToken]
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            if (!httpContext.Items.TryGetValue("AccessToken", out var tokenObj) || tokenObj is not string token || string.IsNullOrEmpty(token))
            {
                context.Result = new ObjectResult(new { message = "Token is missing or invalid." }) { StatusCode = 401 };
                return;
            }

            var (isValid, errorMessage) = await CheckUserRole(token);
            if (!isValid)
            {
                context.Result = new ObjectResult(new { message = errorMessage }) { StatusCode = 403 };
            }
        }

        private async Task<(bool, string?)> CheckUserRole(string token)
        {
            var user = await UserHandler.GetUserByAccessToken(token);
            if (user == null) return (false, "User not found.");

            var role = UserHandler.GetRoleById(user.RoleId);
            if (role == null) return (false, "Role not found.");

            if (!_roles.Contains(role.Name)) return (false, "Access denied.");

            return (true, null);
        }
    }
}
