using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTask.Solver.Db.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.Actions
{
    public class MulticriteriaTaskParameterHeaderCreate
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
            public string UnitOfMeasurement { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.Body)
                    .NotEmpty()
                    .Must(x => !string.IsNullOrWhiteSpace(x.UnitOfMeasurement))
                    .WithMessage($"Для заголовка параметра многокритериальной задачи не указаны единицы измерения. " +
                        $"Заголовок параметра многокритериальной задачи не может быть создан.")
                    .Must(x => !string.IsNullOrWhiteSpace(x.Name))
                    .WithMessage($"Для заголовка параметра многокритериальной задачи не указано имя. " +
                        $"Заголовок параметра многокритериальной задачи не может быть создан.")
                    .Must(x => !dbContext.MulticriteriaTaskParameterHeaders
                        .AsNoTracking()
                        .Any(t => t.Name == x.Name && t.UnitOfMeasurement == x.UnitOfMeasurement))
                    .WithMessage(x => $"Создаваемый заголовок параметра многокритериальной задачи " +
                        $"с именем {x.Body.Name} и единицами измерения {x.Body.UnitOfMeasurement} уже существует и не может быть создан.");
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
                var multicriteriaTaskParameterHeader = new MulticriteriaTaskParameterHeaderModel(
                    request.Body.Name,
                    request.Body.UnitOfMeasurement);

                _dbContext.MulticriteriaTaskParameterHeaders.Add(multicriteriaTaskParameterHeader);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTaskParameterHeader.Id;
            }
        }
    }
}