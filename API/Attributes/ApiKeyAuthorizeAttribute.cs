using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Models;

namespace API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private readonly RoleEnum[] _requiredRoles;

        public ApiKeyAuthorizeAttribute(RoleEnum[] requiredRoles)
        {
            _requiredRoles = requiredRoles;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var extractedApiKey = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "API Key is missing"
                };
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ReservationDbContext>();
            var apiKey = extractedApiKey.Replace("Bearer ", string.Empty);

            var user = await dbContext.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(y => y.ApiKeys.Any(z =>
                                            z.Key == apiKey &&
                                            z.CreatedByUserId == y.Id &&
                                            z.Expiration > DateTime.UtcNow));

            if (user == null ||
                !user.UserRoles.Any(x => _requiredRoles.Contains(x.Role) || x.Role == RoleEnum.Admin))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Unauthorized"
                };
                return;
            }

            context.HttpContext.Items["User"] = user;

            await next();
        }
    }
}
