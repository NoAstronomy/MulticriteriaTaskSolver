using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTasks.Solver.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions
{
    public class CalculateIntersectionSquereTasks
    {
        public class Command : IRequest
        {
            [FromRoute(Name = "typeId")]
            [Required]
            public Guid TypeId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Command>
        {
            public QueryValidator(MulticriteriaTaskSolverDbContext dbContext)
            {
                RuleFor(x => x.TypeId)
                    .NotEmpty()
                    .Must(id => dbContext.MulticriteriaTaskTypes
                        .Any(x => x.Id == id))
                    .WithMessage(id => $"Тип многокритериальной задачи с идентификатором: '{id}' не существует.");
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
                await _multicriteriaTaskService.CalculateIntersectionSquereTasksAsync(request.TypeId);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
