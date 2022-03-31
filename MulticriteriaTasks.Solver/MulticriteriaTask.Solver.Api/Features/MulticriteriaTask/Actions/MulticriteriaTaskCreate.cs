using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTask.Solver.Db.Enums;
using MulticriteriaTask.Solver.Db.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions
{
    public class MulticriteriaTaskCreate
    {
        public class Query : IRequest<Guid>
        {
            [FromBody]
            [Required]
            public Body Body { get; set; }
        }

        public class Body
        {
            [Required]
            public string Name { get; set; }

            [Required]
            public Guid TypeId { get; set; }

            [Required]
            public MulticriteriaTaskStatusEnum Status { get; set; }

            public string Description { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.Body)
                    .NotEmpty()
                    .Must(x => !string.IsNullOrWhiteSpace(x.Name))
                    .WithMessage($"Для многокритериальной задачи не указано имя. " +
                        $"Многокритериальная задача не может быть создана.")
                    .Must(x => !dbContext.MulticriteriaTasks
                        .AsNoTracking()
                        .Any(t => t.Name == x.Name && t.TypeId == x.TypeId))
                    .WithMessage(x => $"Многокритериальная задача" +
                        $"с именем {x.Body.Name} и типом {x.Body.TypeId} уже существует и не может быть создана.");
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
                var taskType = await _dbContext.MulticriteriaTaskTypes
                    .SingleAsync(x => x.Id == request.Body.TypeId, cancellationToken);

                var multicriteriaTask = new MulticriteriaTaskModel(
                    taskType,
                    request.Body.Status,
                    request.Body.Name,
                    request.Body.Description);

                _dbContext.MulticriteriaTasks.Add(multicriteriaTask);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTask.Id;
            }
        }
    }
}