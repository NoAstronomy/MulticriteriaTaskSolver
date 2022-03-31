using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.Actions
{
    public class MulticriteriaParameterDelete
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "parameterId")]
            [Required]
            public Guid ParameterId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.ParameterId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTaskParameterHeaders
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Многокритериальный параметр с идентификатором: '{id}' не существует и не может быть удален.");
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
                var multicriteriaParameter = await _dbContext.MulticriteriaParameters
                    .SingleAsync(x => x.Id == request.ParameterId, cancellationToken);

                _dbContext.Remove(multicriteriaParameter);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaParameter.Id;
            }
        }
    }
}
