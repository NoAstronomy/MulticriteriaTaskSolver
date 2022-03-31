using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTasks.Solver.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions
{
    public class GetTaskConvergenceResult
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "taskId")]
            [Required]
            public Guid TaskId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.TaskId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTasks
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Многритериальная задача с идентификатором: '{id}' не существует.");
            }
        }

        public class Handler : IRequestHandler<Query, Guid>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;
            private readonly IMulticriteriaTaskService _multicriteriaTaskService;

            public Handler(
                MulticriteriaTaskSolverDbContext dbContext,
                IMulticriteriaTaskService multicriteriaTaskService)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _multicriteriaTaskService = multicriteriaTaskService ?? throw new ArgumentNullException(nameof(multicriteriaTaskService));
            }

            public async Task<Guid> Handle(Query request, CancellationToken cancellationToken = default)
            {
                var result = await _multicriteriaTaskService.GetTaskConvergenceResultAsync(request.TaskId);
                return result.Id;
            }
        }
    }
}
