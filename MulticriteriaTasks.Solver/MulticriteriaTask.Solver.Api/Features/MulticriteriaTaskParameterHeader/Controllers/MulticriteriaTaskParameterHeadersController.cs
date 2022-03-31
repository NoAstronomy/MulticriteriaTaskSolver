using MediatR;
using Microsoft.AspNetCore.Mvc;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.Actions;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaTaskParameterHeader.Controllers
{
    [Route("[controller]")]
    public class MulticriteriaTaskParameterHeadersController : Controller
    {
        private readonly IMediator _mediator;

        public MulticriteriaTaskParameterHeadersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Создает заголовок параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskParameterHeaders_MulticriteriaTaskParameterHeaderCreate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Заголовок успешно создан")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPost("[action]")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskParameterHeaderCreate(
            MulticriteriaTaskParameterHeaderCreate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Обновляет заголовок параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskParameterHeaders_MulticriteriaTaskParameterHeaderUpdate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Заголовок успешно обновлен")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPut("[action]/{headerId}")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskParameterHeaderUpdate(
            MulticriteriaTaskParameterHeaderUpdate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Удаляет заголовок параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskParameterHeaders_MulticriteriaTaskParameterHeaderDelete")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Заголовок успешно удален")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpDelete("[action]/{headerId}")]
        public async Task<ActionResult<Guid>> MulticriteriaTaskParameterHeaderDelete(
            MulticriteriaTaskParameterHeaderDelete.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Возвращает заголовок параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskParameterHeaders_MulticriteriaTaskParameterHeaderGet")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Заголовок успешно отдан")]
        [HttpGet("[action]/{headerId}")]
        public async Task<ActionResult<MulticriteriaTaskParameterHeaderBaseViewModel>> MulticriteriaTaskParameterHeaderGet(
            MulticriteriaTaskParameterHeaderGet.Query query, CancellationToken cancellationToken) =>
                Ok(await _mediator.Send(query, cancellationToken));

        /// <summary>
        /// Возвращает список заголовков параметра для многокритериальной задачи
        /// </summary>
        [SwaggerOperation("MulticriteriaTaskParameterHeaders_MulticriteriaTaskParameterHeaderGetList")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Cписок заголовков успешно отдан")]
        [HttpGet("[action]")]
        public async Task<ActionResult<MulticriteriaTaskParameterHeaderBaseViewModel[]>> MulticriteriaTaskParameterHeaderGetList(
            MulticriteriaTaskParameterHeaderGetList.Query query, CancellationToken cancellationToken) =>
                Ok(await _mediator.Send(query, cancellationToken));
    }
}