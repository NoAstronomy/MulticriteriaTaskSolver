using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.ViewModels;
using MulticriteriaTask.Solver.Db;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.Actions
{
    public class MulticriteriaTaskTypeGetList
    {
        public class Query : IRequest<MulticriteriaTaskTypeBaseViewModel[]>
        {
        }

        public class Handler : IRequestHandler<Query, MulticriteriaTaskTypeBaseViewModel[]>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(MulticriteriaTaskSolverDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<MulticriteriaTaskTypeBaseViewModel[]> Handle(Query request, CancellationToken cancellationToken = default) =>
                await _dbContext.MulticriteriaTaskTypes
                    .AsNoTracking()
                    .ProjectTo<MulticriteriaTaskTypeBaseViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);
        }
    }
}