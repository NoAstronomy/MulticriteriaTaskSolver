using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.ViewModels;
using MulticriteriaTask.Solver.Db;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.Actions
{
    public class MulticriteriaParameterGet
    {
        public class Query : IRequest<MulticriteriaParameterBaseViewModel>
        {
            [FromRoute(Name = "parameterId")]
            public Guid ParameterId { get; set; }
        }

        public class Handler : IRequestHandler<Query, MulticriteriaParameterBaseViewModel>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(MulticriteriaTaskSolverDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<MulticriteriaParameterBaseViewModel> Handle(Query request, CancellationToken cancellationToken = default) =>
                await _dbContext.MulticriteriaParameters
                    .AsNoTracking()
                    .Where(x => x.Id == request.ParameterId)
                    .ProjectTo<MulticriteriaParameterBaseViewModel>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}