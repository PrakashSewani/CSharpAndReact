using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Results<Unit>>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Results<Unit>>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this.userAccessor = userAccessor;
                this.context = context;
            }

            public async Task<Results<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities
                    .Include(a=>a.Attendees).ThenInclude(u=>u.AppUser)
                    .FirstOrDefaultAsync(x=>x.Id==request.Id);

                if (activity==null) return null;

                var user = await context.Users
                .FirstOrDefaultAsync(x=>x.UserName==userAccessor.GetUsername());

                if (user==null) return null;
                
                var HostUsername=activity.Attendees.FirstOrDefault(x=>x.IsHost)?.AppUser?.UserName;

                var attendance=activity.Attendees.FirstOrDefault(x=>x.AppUser.UserName==user.UserName);

                if (attendance!=null &&HostUsername==user.UserName)
                    activity.IsCancelled=!activity.IsCancelled;

                if (attendance!=null && HostUsername!=user.UserName)
                    activity.Attendees.Remove(attendance);
                
                if(attendance==null){
                    attendance=new ActivityAttendee{
                       AppUser=user,
                       Activity=activity,
                       IsHost=false 
                    };
                    activity.Attendees.Add(attendance);
                }
                var results=await context.SaveChangesAsync()>0;
                return results? Results<Unit>.Sucess(Unit.Value):Results<Unit>.Failure("Problem Updating your Attendance");
            }
        }
    }
}