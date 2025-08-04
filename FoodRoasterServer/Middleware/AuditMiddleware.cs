using FoodRoasterServer.Services;
using System.Security.Claims;

namespace FoodRoasterServer.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuditService _auditService;

        public AuditMiddleware(RequestDelegate next, IAuditService auditService)
        {
            _next = next;
            _auditService = auditService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User.FindFirst(ClaimTypes.Email)?.Value ?? "Anonymous";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var requestPath = context.Request.Path;

            _auditService.Track($"User: {user}, IP: {ipAddress}, Path: {requestPath}");

            await _next(context);
        }
    }
}
