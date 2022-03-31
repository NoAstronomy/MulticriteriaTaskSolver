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

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.Actions
{
    public class MulticriteriaTaskTypeDelete
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "typeId")]
            [Required]
            public Guid TypeId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.TypeId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTaskTypes
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Тип параметра многокритериальной задачи с идентификатором: '{id}' не существует и не может быть удален.");
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
                var multicriteriaTaskType = await _dbContext.MulticriteriaTaskTypes
                    .SingleAsync(x => x.Id == request.TypeId, cancellationToken);

                _dbContext.Remove(multicriteriaTaskType);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTaskType.Id;
            }
        }
    }
}