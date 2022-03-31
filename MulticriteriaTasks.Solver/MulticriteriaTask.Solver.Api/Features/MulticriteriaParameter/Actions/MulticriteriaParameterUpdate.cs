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

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.Actions
{
    public class MulticriteriaParameterUpdate
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "parameterId")]
            [Required]
            public Guid ParameterId { get; set; }

            [FromBody]
            [Required]
            public Body Body { get; set; }
        }

        public class Body
        {
            [Required]
            public string Name { get; set; }

            [Required]
            public double Value { get; set; }

            [Required]
            public int Order { get; set; }

            [Required]
            public Guid HeaderId { get; set; }

            [Required]
            public Guid MulticriteriaTaskId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.Body)
                    .NotEmpty()
                    .Must(x => dbContext.MulticriteriaTaskParameterHeaders
                        .AsNoTracking()
                        .Any(t => t.Id == x.HeaderId))
                    .WithMessage(x => $"Заголовок параметра многокритериальной задачи с идентификатором {x.Body.HeaderId} не существует. " +
                        $"Заголовок параметра многокритериальной задачи не может быть создан.")
                    .Must(x => dbContext.MulticriteriaTasks
                        .AsNoTracking()
                        .Any(t => t.Id == x.MulticriteriaTaskId))
                    .WithMessage(x => $"Многокритериальная задача с идентификатором {x.Body.MulticriteriaTaskId} не существует. " +
                        $"Многокритериальный параметр не может быть создан.")
                    .Must(x => !string.IsNullOrWhiteSpace(x.Name))
                    .WithMessage($"Для многокритериального параметра не указано имя. " +
                        $"Многокритериальный параметр не может быть создан.");

                RuleFor(x => x)
                    .NotEmpty()
                    .Must(x =>
                    {
                        var existingParameter = dbContext.MulticriteriaParameters
                            .AsNoTracking()
                            .SingleOrDefault(t => t.HeaderId == x.Body.HeaderId
                                && t.Order == x.Body.Order && t.MulticriteriaTaskId == x.Body.MulticriteriaTaskId);

                        return existingParameter == null || existingParameter.Id == x.ParameterId;
                    })
                    .WithMessage(x => $"Многокритериальный параметр" +
                        $"с заголовком {x.Body.HeaderId}, " +
                        $"порядковым номером {x.Body.Order} и многокритериальной задачей {x.Body.MulticriteriaTaskId} " +
                        $"уже существует и не может быть обновлен.");

                RuleFor(x => x.ParameterId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaParameters
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Многокритериальный параметр с идентификатором: '{id}' не существует и не может быть обновлен.");
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
                    .SingleAsync(x => x.Id == request.Body.MulticriteriaTaskId, cancellationToken);

                var header = await _dbContext.MulticriteriaTaskParameterHeaders
                    .SingleAsync(x => x.Id == request.Body.HeaderId, cancellationToken);

                var multicriteriaParameter = await _dbContext.MulticriteriaParameters
                    .SingleAsync(x => x.Id == request.ParameterId, cancellationToken);

                multicriteriaParameter.Update(
                    multicriteriaTask,
                    header,
                    request.Body.Name,
                    request.Body.Value,
                    request.Body.Order);

                await _dbContext.SaveChangesAsync(cancellationToken);
                return multicriteriaParameter.Id;
            }
        }
    }
}