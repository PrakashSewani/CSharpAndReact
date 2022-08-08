using MediatR;
using Domain;
using Persistance;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<Results<ActivityDto>>
        {

            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Results<ActivityDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.mapper = mapper;
                _context = context;
            }

            public async Task<Results<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x=>x.Id==request.Id);

                return Results<ActivityDto>.Sucess(activity);
            }
        }
    }

}

