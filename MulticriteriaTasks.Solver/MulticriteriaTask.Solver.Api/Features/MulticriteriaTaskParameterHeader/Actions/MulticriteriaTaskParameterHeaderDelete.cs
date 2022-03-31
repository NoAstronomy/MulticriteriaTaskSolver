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
    public class MulticriteriaTaskParameterHeaderDelete
    {
        public class Query : IRequest<Guid>
        {
            [FromRoute(Name = "headerId")]
            [Required]
            public Guid HeaderId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.HeaderId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTaskParameterHeaders
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Заголовок параметра многокритериальной задачи с идентификатором: '{id}' не существует и не может быть удален.");
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

                _dbContext.Remove(multicriteriaTaskParameterHeader);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return multicriteriaTaskParameterHeader.Id;
            }
        }
    }
}