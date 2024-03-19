using Marcet_Api_V2.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Net;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        bool isAuthorizedEndpoint = IsAuthorizedEndpoint(endpoint);

        if (isAuthorizedEndpoint)
        {
            string? currentUserId = GetCurrentUserId(context);

            if (currentUserId.IsNullOrEmpty())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Invalid access token, please relogin");
                return;
            }

            context.SetCurrentUserId(currentUserId);
        }

        await _next(context);
    }

    private string? GetCurrentUserId(HttpContext context)
    {
        return context.User.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }

    private bool IsAuthorizedEndpoint(Endpoint endpoint)
    {
        if (endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>() is ControllerActionDescriptor descriptor)
        {
            return descriptor.ControllerTypeInfo
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Any();
        }

        if (endpoint?.Metadata.GetMetadata<HubMetadata>() != null)
        {
            return true;
        }

        return false;
    }
}