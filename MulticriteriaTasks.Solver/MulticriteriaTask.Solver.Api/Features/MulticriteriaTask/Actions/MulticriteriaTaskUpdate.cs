using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTask.Solver.Db.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions
{
    public class MulticriteriaTaskUpdate
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "taskId")]
            [Required]
            public Guid TaskId { get; set; }

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
                        $"Многокритериальная задача не может быть создан.")
                    .Must(x => !dbContext.MulticriteriaTasks
                        .AsNoTracking()
                        .Any(t => t.Name == x.Name && t.TypeId == x.TypeId))
                    .WithMessage(x => $"Многокритериальная задача" +
                        $"с именем {x.Body.Name} и типом {x.Body.TypeId} уже существует и не может быть создан.");

                RuleFor(x => x)
                    .NotEmpty()
                    .Must(x =>
                    {
                        var existingTask = dbContext.MulticriteriaTasks
                            .AsNoTracking()
                            .SingleOrDefault(t => t.Name == x.Body.Name && t.TypeId == x.Body.TypeId);

                        return existingTask == null || existingTask.Id == x.TaskId;
                    })
                    .WithMessage(x => $"Многокритериальная задача " +
                        $"с именем {x.Body.Name} и типом {x.Body.TypeId} уже существует и не может быть обновлена.");

                RuleFor(x => x.TaskId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTasks
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Многокритериальная задача с идентификатором: '{id}' не существует и не может быть обновлена.");
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

                var multicriteriaTask = await _dbContext.MulticriteriaTasks
                    .SingleAsync(x => x.Id == request.TaskId, cancellationToken);

                multicriteriaTask.Update(
                    taskType,
                    request.Body.Status,
                    request.Body.Name,
                    request.Body.Description);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTask.Id;
            }
        }
    }
}