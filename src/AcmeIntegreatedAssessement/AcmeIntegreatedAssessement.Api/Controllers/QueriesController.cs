using System;
using AcmeIntegreatedAssessement.Application.Queries.GetPaymentState;
using AcmeIntegreatedAssessement.Application.Queries.GetDailyPaymentSummary;
using AcmeIntegreatedAssessement.Api.Contracts;
using AcmeIntegreatedAssessement.Application.Contracts;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class QueriesController : ControllerBase
    {
        private readonly ISender _mediator;

        public QueriesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified int.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("payments/{appPaymentId}")]
        [ProducesResponseType(typeof(MessageEnvelope<PaymentStateResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MessageEnvelope<PaymentStateResponseDto>>> GetPaymentState(
            [FromRoute] string appPaymentId,
            [FromQuery] string channel = "App",
            CancellationToken cancellationToken = default)
        {
            if (!ApiMessageEnvelope.IsValidChannel(channel))
            {
                return BadRequest();
            }

            var result = await _mediator.Send(new GetPaymentStateQuery(appPaymentId: appPaymentId), cancellationToken);
            return Ok(ApiMessageEnvelope.Create(result, channel));
        }

        [HttpGet("reports/daily-summary")]
        [ProducesResponseType(typeof(MessageEnvelope<DailyPaymentSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MessageEnvelope<DailyPaymentSummaryResponse>>> GetDailyPaymentSummary(
            [FromQuery] DateTime? reportDate = null,
            [FromQuery] string channel = "App",
            CancellationToken cancellationToken = default)
        {
            if (!ApiMessageEnvelope.IsValidChannel(channel))
            {
                return BadRequest();
            }

            var effectiveDate = reportDate ?? DateTime.UtcNow.AddHours(2);
            var dateOnly = DateOnly.FromDateTime(effectiveDate.Date);
            var result = await _mediator.Send(new GetDailyPaymentSummaryQuery(dateOnly), cancellationToken);
            return Ok(ApiMessageEnvelope.Create(result, channel));
        }
    }
}