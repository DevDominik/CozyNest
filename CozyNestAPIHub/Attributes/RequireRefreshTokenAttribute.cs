﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;
using CozyNestAPIHub.Handlers;

namespace CozyNestAPIHub.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireRefreshTokenAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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

            if (!await UserHandler.ValidateRefreshToken(token))
            {
                context.Result = new ObjectResult(new { message = "Invalid refresh token." }) { StatusCode = 403 };
                return;
            }
            httpContext.Items["RefreshToken"] = token;

            await next();
        }
    }
}
