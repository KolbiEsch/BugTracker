using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BugTracker.Data.Authorization
{
    public class DocumentAuthorizationHandler : AuthorizationHandler<AssignedTicketRequirement, Ticket>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                      AssignedTicketRequirement requirement,
                                                      Ticket resource)
        {
            if (context.User.HasClaim(ClaimTypes.NameIdentifier, resource.ApplicationUserId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
