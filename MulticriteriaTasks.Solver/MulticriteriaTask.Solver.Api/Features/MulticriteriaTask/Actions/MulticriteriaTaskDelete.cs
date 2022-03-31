using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions
{
    public class MulticriteriaTaskDelete
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
                    .Must(id => dbContext.MulticriteriaTaskParameterHeaders
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Многокритериальная задача с идентификатором: '{id}' не существует и не может быть удалена.");
            }
        }

        public class Handler : IRequestHandler<Query, Guid>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;

            public Handler(MulticriteriaTaskSolverDbContext dbContext)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            public async Task<Guid> Handle(Query request, CancellationToken cancellationToken = default)
            {
                var multicriteriaTask = await _dbContext.MulticriteriaTasks
                    .SingleAsync(x => x.Id == request.TaskId, cancellationToken);

                _dbContext.Remove(multicriteriaTask);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTask.Id;
            }
        }
    }
}