using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MulticriteriaTask.Solver.Db.Models;
using FluentValidation;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.Actions
{
    public class MulticriteriaTaskTypeCreate
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
            public string Description { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.Body)
                    .NotEmpty()
                    .Must(x => !string.IsNullOrWhiteSpace(x.Name))
                    .WithMessage(x => $"Для создаваемого типа многокритериальной задачи не указано имя. " +
                        $"Тип многокритериальной задачи не может быть создан.")
                    .Must(x => !dbContext.MulticriteriaTaskTypes
                        .AsNoTracking()
                        .Any(t => t.Name == x.Name))
                    .WithMessage(x => $"Создаваемый тип параметра многокритериальной задачи с именем {x.Body.Name} уже существует и не может быть создан.");
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
                var multicriteriaTaskType = new MulticriteriaTaskTypeModel(request.Body.Name, request.Body.Description);

                _dbContext.MulticriteriaTaskTypes.Add(multicriteriaTaskType);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTaskType.Id;
            }
        }
    }
}