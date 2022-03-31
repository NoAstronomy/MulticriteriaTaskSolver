using MediatR;
using Microsoft.AspNetCore.Mvc;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.Actions;
using MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MulticriteriaTask.Solver.Api.Features.MulticriteriaParameter.Controllers
{
    [Route("[controller]")]
    public class MulticriteriaParametersController : Controller
    {
        private readonly IMediator _mediator;

        public MulticriteriaParametersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Создает мультикритериальный параметр
        /// </summary>
        [SwaggerOperation("MulticriteriaParameters_MulticriteriaParameterСreate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Мультикритериальный параметр успешно создан")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPost("[action]")]
        public async Task<ActionResult<Guid[]>> MulticriteriaParameterСreate(
            MulticriteriaParameterСreate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Обновляет мультикритериальный параметр
        /// </summary>
        [SwaggerOperation("MulticriteriaParameters_MulticriteriaParameterUpdate")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Мультикритериальный параметр успешно обновлен")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpPut("[action]/{parameterId}")]
        public async Task<ActionResult<Guid>> MulticriteriaParameterUpdate(
            MulticriteriaParameterUpdate.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Удаляет мультикритериальный параметр
        /// </summary>
        [SwaggerOperation("MulticriteriaParameters_MulticriteriaParameterDelete")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Мультикритериальный параметр успешно удален")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Отправлены некорректные данные")]
        [HttpDelete("[action]/{parameterId}")]
        public async Task<ActionResult<Guid>> MulticriteriaParameterDelete(MulticriteriaParameterDelete.Query query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Возвращает мультикритериальный параметр
        /// </summary>
        [SwaggerOperation("MulticriteriaParameters_MulticriteriaParameterGet")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Мультикритериальный параметр успешно отдан")]
        [HttpGet("[action]/{parameterId}")]
        public async Task<ActionResult<MulticriteriaParameterBaseViewModel>> MulticriteriaParameterGet(
            MulticriteriaParameterGet.Query query, CancellationToken cancellationToken) =>
                Ok(await _mediator.Send(query, cancellationToken));

        ///// <summary>
        ///// Возвращает список мультикритериальных параметров
        ///// </summary>
        //[SwaggerOperation("MulticriteriaParameters_MulticriteriaParameterGetList")]
        //[SwaggerResponse((int)HttpStatusCode.OK, "Cписок мультикритериальных параметров успешно отдан")]
        //[HttpPost("[action]")]
        //public async Task<ActionResult<MulticriteriaParameterGetList.MulticriteriaParameterViewModel[]>> MulticriteriaParameterGetList(
        //    MulticriteriaParameterGetList.Query query, CancellationToken cancellationToken) =>
        //        Ok(await _mediator.Send(query, cancellationToken));
    }
}