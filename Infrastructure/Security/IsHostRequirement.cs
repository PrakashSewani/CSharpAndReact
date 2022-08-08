using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {

    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly DataContext dbcontext;
        private readonly IHttpContextAccessor hTTPContextAccessor;
        public IsHostRequirementHandler(DataContext dbcontext, 
            IHttpContextAccessor hTTPContextAccessor)
        {
            this.hTTPContextAccessor = hTTPContextAccessor;
            this.dbcontext = dbcontext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            var userid=context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userid==null) return Task.CompletedTask;

            var activityid=Guid.Parse(hTTPContextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x=>x.Key=="id").Value?.ToString());

            var attendee=dbcontext.ActivityAttendees
                .AsNoTracking()
                .SingleOrDefaultAsync(x=>x.AppUserId==userid && x.ActivityId==activityid ).Result;

            if (attendee==null) return Task.CompletedTask;

            if (attendee.IsHost) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}