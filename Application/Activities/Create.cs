using MediatR;
using Domain;
using Persistance;
using FluentValidation;
using Application.Core;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest<Results<Unit>>
        {
            public Activity Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Results<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this.userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Results<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {

                var user=await _context.Users.FirstOrDefaultAsync(x=>x.UserName==
                userAccessor.GetUsername());

                var attendee= new ActivityAttendee{
                    AppUser=user,
                    Activity=request.Activity,
                    IsHost=true
                };

                request.Activity.Attendees.Add(attendee);

                _context.Activities.Add(request.Activity);

                var results = await _context.SaveChangesAsync() > 0;

                if (!results) return Results<Unit>.Failure("Failed To Create Activity");

                return Results<Unit>.Sucess(Unit.Value);
            }
        }
    }
}