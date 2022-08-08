using MediatR;
using Domain;
using AutoMapper;
using Persistance;
using FluentValidation;
using Application.Core;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest<Results<Unit>>
        {
            public Activity Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x=>x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command,Results<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Results<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                if(activity==null) return null;

                _mapper.Map(request.Activity,activity);

                var result=await _context.SaveChangesAsync()>0;

                if(!result) return Results<Unit>.Failure("Failed To Update Activity");

                return Results<Unit>.Sucess(Unit.Value);
            }
        }
    }
}