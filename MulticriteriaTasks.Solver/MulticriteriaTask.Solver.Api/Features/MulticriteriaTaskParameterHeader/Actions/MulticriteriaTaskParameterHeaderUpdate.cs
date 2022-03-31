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

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.Actions
{
    public class MulticriteriaTaskParameterHeaderUpdate
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "headerId")]
            [Required]
            public Guid HeaderId { get; set; }

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
                        $"Заголовок параметра многокритериальной задачи не может быть создан.");

                RuleFor(x => x)
                    .NotEmpty()
                    .Must(x => 
                    {
                        var existingHeader = dbContext.MulticriteriaTaskParameterHeaders
                            .AsNoTracking()
                            .SingleOrDefault(t => t.Name == x.Body.Name && t.UnitOfMeasurement == x.Body.UnitOfMeasurement);

                        return existingHeader == null || existingHeader.Id == x.HeaderId;
                    })
                    .WithMessage(x => $"Заголовок параметра многокритериальной задачи " +
                        $"с именем {x.Body.Name} и единицами измерения {x.Body.UnitOfMeasurement} уже существует и не может быть обновлен.");

                RuleFor(x => x.HeaderId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTaskParameterHeaders
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Тип параметра многокритериальной задачи с идентификатором: '{id}' не существует и не может быть обновлен.");
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
                var multicriteriaTaskParameterHeader = await _dbContext.MulticriteriaTaskParameterHeaders
                    .SingleAsync(x => x.Id == request.HeaderId, cancellationToken);

                multicriteriaTaskParameterHeader.Update(
                    request.Body.Name, 
                    request.Body.UnitOfMeasurement);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTaskParameterHeader.Id;
            }
        }
    }
}