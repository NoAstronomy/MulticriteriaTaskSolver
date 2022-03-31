using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.ViewModels;
using MulticriteriaTask.Solver.Db;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions
{
    public class MulticriteriaTaskGet
    {
        public class Query : IRequest<MulticriteriaTaskBaseViewModel>
        {
            [FromRoute(Name = "taskId")]
            public Guid TaskId { get; set; }
        }

        public class Handler : IRequestHandler<Query, MulticriteriaTaskBaseViewModel>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(MulticriteriaTaskSolverDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<MulticriteriaTaskBaseViewModel> Handle(Query request, CancellationToken cancellationToken = default) =>
                await _dbContext.MulticriteriaTasks
                    .AsNoTracking()
                    .Where(x => x.Id == request.TaskId)
                    .ProjectTo<MulticriteriaTaskBaseViewModel>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}