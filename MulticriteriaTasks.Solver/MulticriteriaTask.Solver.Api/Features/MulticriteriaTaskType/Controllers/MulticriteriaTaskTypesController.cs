using MediatR;
using Microsoft.AspNetCore.Mvc;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.Actions;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.Actions;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskType.Controllers
{
    [Route("[controller]")]
    public class MulticriteriaTaskTypesController : Controller
    {
        private readonly IMediator _mediator;

        public MulticriteriaTaskTypesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Создает тип параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskTypes_MulticriteriaTaskTypeCreate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Тип успешно создан")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPost("[action]")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskTypeCreate(
            MulticriteriaTaskTypeCreate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Обновляет тип параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskTypes_MulticriteriaTaskTypeUpdate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Тип успешно обновлен")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPut("[action]/{typeId}")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskTypeUpdate(
            MulticriteriaTaskTypeUpdate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Удаляет тип параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskTypes_MulticriteriaTaskTypeDelete")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Тип успешно удален")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpDelete("[action]/{typeId}")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskTypeDelete(
            MulticriteriaTaskTypeDelete.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Возвращает тип параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskTypes_MulticriteriaTaskTypeGet")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Тип успешно отдан")]
        [HttpGet("[action]/{typeId}")]
        public async Task<ActionResult<MulticriteriaTaskTypeBaseViewModel>> MulticriteriaTaskTypeGet(
            MulticriteriaTaskTypeGet.Query query, CancellationToken cancellationToken) =>
                Ok(await _mediator.Send(query, cancellationToken));

        /// <summary>
        /// Возвращает список типов параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskTypes_MulticriteriaTaskTypeGetList")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Cписок типов успешно отдан")]
        [HttpGet("[action]")]
        public async Task<ActionResult<MulticriteriaTaskTypeBaseViewModel[]>> MulticriteriaTaskTypeGetList(
            MulticriteriaTaskTypeGetList.Query query, CancellationToken cancellationToken) =>
                Ok(await _mediator.Send(query, cancellationToken));
    }
}