using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles payments operations 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,invoice entry,payment entry")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;
        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a list of suggested payments that are ready to be paid
        /// </summary>
        /// <response code="200">returns list of suggested payments</response>
        [HttpGet("paymentstopay")]
        [ProducesResponseType(typeof(List<ProjectInvoicePaymentReadyToPayDto>), 200)]
        public async Task<ActionResult<List<ProjectInvoicePaymentReadyToPayDto>?>> PaymentsToPay()
        {
            var payments = await _service.GetProjectInvoicePaymentReadyToPayAsync();
            return Ok(payments);
        }

        /// <summary>
        /// Retrieves a list of suggested payments by a list of payments Ids
        /// </summary>
        /// <response code="200">returns list of suggested payments</response>
        [HttpPost("PaymentsToPayByIds")]
        [ProducesResponseType(typeof(List<ProjectInvoicePaymentReadyToPayDto>), 200)]
        public async Task<ActionResult<List<ProjectInvoicePaymentReadyToPayDto>?>> PaymentsToPayByIds(
            [FromBody] List<int> paymentsIds)
        {
            var payments = await _service.GetProjectInvoicePaymentsByIdsAsync(paymentsIds);
            return Ok(payments);
        }

        /// <summary>
        /// Pay a project invoice suggested payment and a add relevant bank checks and cash info
        /// </summary>
        /// <response code="404">project invoice suggested payment not found</response>
        /// <response code="400">payment info are not valid</response>
        /// <response code="204">pay succeeded</response>
        [HttpPost("Payment")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Payment([FromBody] ProjectInvoicePaymentCreationDto paymentDto)
        {
            await _service.PayPaymentAsync(paymentDto);
            return NoContent();
        }

        /// <summary>
        /// Pay a list of project invoice suggested payment and a add relevant bank checks and cash info
        /// </summary>
        /// <response code="404">list of project invoice suggested payments not found</response>
        /// <response code="400">list of payments info are not valid</response>
        /// <response code="204">list of suggested payments are paid successfully</response>
        [HttpPost("paymentgroup")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PaymentGroup([FromBody] ProjectInvoiceGroupPaymentCreationDto paymentDto)
        {
            await _service.PayPaymentGroupAsync(paymentDto);
            return NoContent();
        }
    }
}
