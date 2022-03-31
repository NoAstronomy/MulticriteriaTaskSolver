using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTasks.Solver.Services.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions
{
    public class MulticriteriaTaskCalculateSquere
    {
        public class Command : IRequest
        {
            [FromRoute(Name = "taskId")]
            [Required]
            public Guid TaskId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Command>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.TaskId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTasks
                        .AsNoTracking()
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Многокритериальная задача с идентификатором: '{id}' не существует и не может быть обновлена.");
            }
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly MulticriteriaTaskSolverDbContext _dbContext;
            private readonly IMulticriteriaTaskService _multicriteriaTaskService;

            public Handler(
                MulticriteriaTaskSolverDbContext dbContext,
                IMulticriteriaTaskService multicriteriaTaskService)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _multicriteriaTaskService = multicriteriaTaskService ?? throw new ArgumentNullException(nameof(multicriteriaTaskService));
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken = default)
            {
                await _multicriteriaTaskService.CalculateSquereBySegmentsAsync(request.TaskId);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}