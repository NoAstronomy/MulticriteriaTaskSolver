using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.ViewModels;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTask.Solver.Db.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.Actions
{
    public class MulticriteriaTaskParameterHeaderGetList
    {
        public class Query : IRequest<MulticriteriaTaskParameterHeaderBaseViewModel[]>
        {
        }

        public class Handler : IRequestHandler<Query, MulticriteriaTaskParameterHeaderBaseViewModel[]>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(MulticriteriaTaskSolverDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<MulticriteriaTaskParameterHeaderBaseViewModel[]> Handle(Query request, CancellationToken cancellationToken = default) =>
                await _dbContext.MulticriteriaTaskParameterHeaders
                    .AsNoTracking()
                    .ProjectTo<MulticriteriaTaskParameterHeaderBaseViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);
        }
    }
}