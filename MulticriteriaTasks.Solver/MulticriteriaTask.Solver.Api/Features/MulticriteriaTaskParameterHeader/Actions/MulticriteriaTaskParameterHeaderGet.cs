using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.ViewModels;
using MulticriteriaTask.Solver.Db;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.Actions
{
    public class MulticriteriaTaskParameterHeaderGet
    {
        public class Query : IRequest<MulticriteriaTaskParameterHeaderBaseViewModel>
        {
            [FromRoute(Name = "headerId")]
            public Guid HeaderId { get; set; }
        }

        public class Handler : IRequestHandler<Query, MulticriteriaTaskParameterHeaderBaseViewModel>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(MulticriteriaTaskSolverDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<MulticriteriaTaskParameterHeaderBaseViewModel> Handle(Query request, CancellationToken cancellationToken = default) =>
                await _dbContext.MulticriteriaTaskParameterHeaders
                    .AsNoTracking()
                    .Where(x => x.Id == request.HeaderId)
                    .ProjectTo<MulticriteriaTaskParameterHeaderBaseViewModel>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}