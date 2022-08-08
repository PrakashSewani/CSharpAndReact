using MediatR;
using Domain;
using Persistance;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Results<List<ActivityDto>>>
        {

        }

        public class Handler : IRequestHandler<Query, Results<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this.mapper = mapper;
                _context = context;

            }

            public override bool Equals(object? obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public async Task<Results<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = await _context.Activities
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .ToListAsync();

                return Results<List<ActivityDto>>.Sucess(activities);
            }

            public override string? ToString()
            {
                return base.ToString();
            }
        }
    }
}