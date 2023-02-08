using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AuctionSite.AuthorizationHandlers
{
    public class paidRequirement : AuthorizationHandler<paidRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, paidRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "paid"))
                context.Succeed(requirement);
            else
                context.Fail();
            return Task.CompletedTask;
        }
}
}

