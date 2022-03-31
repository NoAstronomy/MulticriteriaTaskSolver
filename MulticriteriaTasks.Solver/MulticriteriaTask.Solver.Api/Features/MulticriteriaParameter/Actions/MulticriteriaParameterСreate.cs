using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTask.Solver.Db.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.Actions
{
    public class MulticriteriaParameterСreate
    {
        public class Query : IRequest<Guid[]>
        {
            [FromBody]
            [Required]
            public Body Body { get; set; }
        }

        public class Body
        {
            [Required]
            public MulticriteriaParameterBody[] MulticriteriaParameters { get; set; }
        }

        public class MulticriteriaParameterBody
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
                    .NotEmpty();

                RuleForEach(ch => ch.Body.MulticriteriaParameters)
                    .SetValidator(new MulticriteriaParameterBodyValidator(dbContext));
            }
        }

        public class MulticriteriaParameterBodyValidator : AbstractValidator<MulticriteriaParameterBody>
        {
            public MulticriteriaParameterBodyValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .Must(x => dbContext.MulticriteriaTaskParameterHeaders
                        .AsNoTracking()
                        .Any(t => t.Id == x.HeaderId))
                    .WithMessage(x => $"Заголовок параметра многокритериальной задачи с идентификатором {x.HeaderId} не существует. " +
                        $"Многокритериальный параметр не может быть создан.")
                    .Must(x => dbContext.MulticriteriaTasks
                        .AsNoTracking()
                        .Any(t => t.Id == x.MulticriteriaTaskId))
                    .WithMessage(x => $"Многокритериальная задача с идентификатором {x.MulticriteriaTaskId} не существует. " +
                        $"Многокритериальный параметр не может быть создан.")
                    .Must(x => !string.IsNullOrWhiteSpace(x.Name))
                    .WithMessage($"Для многокритериального параметра не указано имя. " +
                        $"Многокритериальный параметр не может быть создан.")
                    .Must(x => !dbContext.MulticriteriaParameters
                        .AsNoTracking()
                        .Any(t => t.HeaderId == x.HeaderId && t.MulticriteriaTaskId == x.MulticriteriaTaskId && t.Order == x.Order))
                    .WithMessage(x => $"Создаваемый многокритериальный параметр " +
                        $"с типом {x.HeaderId}, порядковым номером {x.Order} и многокритериальной задачей {x.MulticriteriaTaskId}" +
                        $"уже существует и не может быть создан.");
            }
        }

        public class Handler : IRequestHandler<Query, Guid[]>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;

            public Handler(MulticriteriaTaskSolverDbContext dbContext)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            public async Task<Guid[]> Handle(Query request, CancellationToken cancellationToken = default)
            {
                var parameters = request.Body.MulticriteriaParameters.ToArray();

                var results = new List<Guid>();
                foreach (var parameter in parameters)
                {
                    var multicriteriaTask = await _dbContext.MulticriteriaTasks
                        .SingleAsync(x => x.Id == parameter.MulticriteriaTaskId, cancellationToken);

                    var header = await _dbContext.MulticriteriaTaskParameterHeaders
                        .SingleAsync(x => x.Id == parameter.HeaderId, cancellationToken);

                    var multicriteriaParameter = new MulticriteriaParameterModel(
                        multicriteriaTask,
                        header,
                        parameter.Name,
                        parameter.Value,
                        parameter.Order);

                    _dbContext.MulticriteriaParameters.Add(multicriteriaParameter);
                    results.Add(multicriteriaParameter.Id);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                return results.ToArray();
            }
        }
    }
}