using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Meritocious.Core.Entities;

namespace Meritocious.Web.Authorization
{
    public class GoogleLinkedHandler : AuthorizationHandler<GoogleLinkedRequirement>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleLinkedHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GoogleLinkedRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
                return;

            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
                return;

            var logins = await _userManager.GetLoginsAsync(user);
            bool isGoogleLinked = logins.Any(l => l.LoginProvider == "Google");

            if (isGoogleLinked)
            {
                context.Succeed(requirement);
            }
            else
            {
                httpContext.Response.Redirect("/Account/LinkGoogle");
                context.Fail();
            }
        }
    }
}