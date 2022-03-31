using MediatR;
using Microsoft.AspNetCore.Mvc;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Actions;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTask.Controllers
{
    [Route("[controller]")]
    public class MulticriteriaTasksController : Controller
    {
        private readonly IMediator _mediator;

        public MulticriteriaTasksController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Создает многокритериальную задачу
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_MulticriteriaTaskCreate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Многокритериальная задача успешно создана")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPost("[action]")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskCreate(
            MulticriteriaTaskCreate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Обновляет многокритериальную задачу
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_MulticriteriaTaskUpdate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Многокритериальная задача успешно обновлена")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPut("[action]/{taskId}")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskUpdate(
            MulticriteriaTaskUpdate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Удаляет многокритериальную задачу
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_MulticriteriaTaskDelete")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Многокритериальная задача успешно удалена")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpDelete("[action]/{taskId}")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskDelete(
            MulticriteriaTaskDelete.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Возвращает многокритериальную задачу
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_MulticriteriaTaskGet")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Многокритериальная задача успешно отдана")]
        [HttpGet("[action]/{taskId}")]
        public async Task<ActionResult<MulticriteriaTaskBaseViewModel>> MulticriteriaTaskGet(
            MulticriteriaTaskGet.Query query, CancellationToken cancellationToken) =>
                Ok(await _mediator.Send(query, cancellationToken));

        /// <summary>
        /// Возвращает список многокритериальных задач
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_MulticriteriaTaskGetList")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Cписок многокритериальных задач успешно отдан")]
        [HttpGet("[action]")]
        public async Task<ActionResult<MulticriteriaTaskBaseViewModel[]>> MulticriteriaTaskGetList(
            MulticriteriaTaskGetList.Query query, CancellationToken cancellationToken) =>
                Ok(await _mediator.Send(query, cancellationToken));

        /// <summary>
        /// Вычисляет координаты для параметров многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_CalculateParameterCoordinates")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Ккординаты для параметров успешно вычислены")]
        [HttpPut("[action]/{taskId}")]
        public async Task<IActionResult> CalculateParameterCoordinates(
            CalculateParameterCoordinates.Command query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Расчет отрезков, которые образуют точки параметров многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_CalculateSegmentsFromTaskParameterPoints")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Отрезки успешно рассчитаны")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPut("[action]/{taskId}")]
        public async Task<IActionResult> CalculateSegmentsFromTaskParameterPoints(
            CalculateSegmentsFromTaskParameterPoints.Command query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Вычисление площади многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_MulticriteriaTaskCalculateSquere")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Площадь успешно вычислена")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPut("[action]/{taskId}")]
        public async Task<IActionResult> MulticriteriaTaskCalculateSquere(
            MulticriteriaTaskCalculateSquere.Command query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Расчет площади пересечения многокритериальных задач связанных по типу
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_CalculateIntersectionSquereTasks")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Площадь пересечения успешно вычислена")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPut("[action]/{typeId}")]
        public async Task<IActionResult> CalculateIntersectionSquereTasks(
            CalculateIntersectionSquereTasks.Command query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Результат сходимости исследуемой задачи к базовым задачам одного типа
        /// </summary>
        [SwaggerOperation("MulticriteriaTasks_GetTaskConvergenceResultAsync")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Результат сходимости успешно получен")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpGet("[action]/{taskId}")]
        public async Task<ActionResult<Guid>> GetTaskConvergenceResult(
            GetTaskConvergenceResult.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}