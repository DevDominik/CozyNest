using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CozyNestAPIHub.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace CozyNestAPIHub.Attributes
{
    public class RoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request;

            if (token == null || ExtractTokenFromBody(token) == null)
            {
                context.Result = new ObjectResult(new { message = "Token is missing." }) { StatusCode = 400 };
                return;
            }

            var tokenString = await ExtractTokenFromBody(token);
            if (string.IsNullOrEmpty(tokenString))
            {
                context.Result = new ObjectResult(new { message = "Token is missing." }) { StatusCode = 400 };
                return;
            }

            var (isValid, errorMessage) = await Check(tokenString);
            if (!isValid)
            {
                context.Result = new ObjectResult(new { message = errorMessage }) { StatusCode = 403 };
            }
        }

        private async Task<string?> ExtractTokenFromBody(HttpRequest request)
        {
            if (!request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
            {
                return null;
            }

            request.EnableBuffering();

            using (var reader = new System.IO.StreamReader(request.Body, System.Text.Encoding.UTF8, leaveOpen: true))
            {
                string bodyContent = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                if (string.IsNullOrWhiteSpace(bodyContent)) return null;

                try
                {
                    var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(bodyContent);
                    foreach (var property in jsonObject.Properties())
                    {
                        if (property.Name.ToLower() == "accesstoken")
                        {
                            return property.Value.ToString();
                        }
                    }

                    return null;
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    return null;
                }
            }
        }


        private async Task<(bool, string?)> Check(string token)
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
