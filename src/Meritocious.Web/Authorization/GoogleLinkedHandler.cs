using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Meritocious.Core.Entities;

namespace Meritocious.Web.Authorization
{
    public class GoogleLinkedHandler : AuthorizationHandler<GoogleLinkedRequirement>
    {
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GoogleLinkedHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GoogleLinkedRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            var httpContext = httpContextAccessor.HttpContext;
            var user = await userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            var logins = await userManager.GetLoginsAsync(user);
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