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
    public class MulticriteriaTaskTypeUpdate
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "typeId")]
            [Required]
            public Guid TypeId { get; set; }

            [FromBody]
            [Required]
            public Body Body { get; set; }
        }

        public class Body
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
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
                    .WithMessage(id => $"Тип параметра многокритериальной задачи с идентификатором: '{id}' не существует и не может быть обновлен.");

                RuleFor(x => x.Body)
                    .Must(x => !string.IsNullOrWhiteSpace(x.Name))
                    .WithMessage(x => $"Для обновляемого типа многокритериальной задачи не указано имя. " +
                        $"Тип многокритериальной задачи не может быть обновлен.")
                    .NotEmpty();

                RuleFor(x => x)
                    .NotEmpty()
                    .Must(x =>
                    {
                        var existingType = dbContext.MulticriteriaTaskTypes
                            .AsNoTracking()
                            .SingleOrDefault(t => t.Name == x.Body.Name);

                        return existingType == null || (existingType.Id == x.TypeId);
                    })
                    .WithMessage(x => $"Тип параметра многокритериальной задачи с именем {x.Body.Name} уже существует и не может быть обновлен.");
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

                multicriteriaTaskType.Update(
                    request.Body.Name,
                    request.Body.Description);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTaskType.Id;
            }
        }
    }
}