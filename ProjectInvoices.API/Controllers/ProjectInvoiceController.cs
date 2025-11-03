using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TakalNew.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Domain.IRepository;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to project invoices, including:
    /// add new invoice, update existing invoice, approve invoice, get list of invoice suggested payments
    /// pay one payment, pay group of payments 
    /// </summary>
    [Route("api/projectinvoice")]
    [ApiController]
    [Authorize(Roles = "admin,invoice entry,payment entry")]
    public class ProjectInvoiceController : ControllerBase
    {
        private readonly IProjectInvoiceService _service;
        private readonly IProjectInvoiceQueries _queries;
        private readonly IMapper _mapper;

        public ProjectInvoiceController(IProjectInvoiceService service, IProjectInvoiceQueries queries, IMapper mapper)
        {
            _service = service;
            _queries = queries;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new project invoice
        /// </summary>
        /// <param name="projectInvoiceDto">project invoice info</param>
        /// <returns>no content if invoice created successfully</returns>
        /// <response code="400">invoice info are not valid</response>
        /// <response code="204">invoice created successfully</response>
        /// 
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectInvoiceCreationDto projectInvoiceDto)
        {

            var projectInvoice = _mapper.Map<ProjectInvoice>(projectInvoiceDto);
            
            //Add new project invoice to DB
            await _service.AddProjectInvoiceAsync(projectInvoice);
            
            return NoContent();
        }

        /// <summary>
        /// Retrieves a project invoice by its id
        /// </summary>
        /// <param name="id">project invoice id</param>
        /// <returns>the matching project invoice dto object</returns>
        /// <response code="404">project invoice not found</response>
        /// <response code="200">returns the matching project invoice dto object</response>
        /// 
        [HttpGet("putget/{id:int}")]
        public async Task<ActionResult<ProjectInvoiceUpdateGetDto>> PutGet(int id)
        {
            //Get project invoice with its items from DB by its id
            var projectInvoice = await _service.GetProjectInvoiceWithItemsByIdAsync(id);
            if (projectInvoice == null)
            {
                return NotFound();
            }

            var result = new ProjectInvoiceUpdateGetDto();
            _mapper.Map(projectInvoice, result);
            
            return Ok(result);
        }

        /// <summary>
        /// update a project invoice
        /// </summary>
        /// <param name="id">project invoice id</param>
        /// <param name="projectInvoiceUpdateDto">project invoice info</param>
        /// <returns>no content if project invoice updated successfully</returns>
        /// <response code="404">project invoice not found</response>
        /// <response code="400">project invoice info are not valid</response>
        /// <response code="204">project invoice updated successfully</response>
        ///
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProjectInvoiceUpdateDto projectInvoiceUpdateDto)
        {
            //Get project invoice with its items from DB by its id
            var projectInvoice = await _service.GetProjectInvoiceWithItemsByIdAsync(id);
            if (projectInvoice == null)
            {
                return NotFound();
            }

            _mapper.Map(projectInvoiceUpdateDto, projectInvoice);
            
            //Update project invoice in DB
            await _service.UpdateProjectInvoiceAsync();

            return NoContent();
        }

        /// <summary>
        /// Approve a project invoice
        /// </summary>
        /// <param name="id">project invoice id</param>
        /// <returns>no content if project invoice approved successfully</returns>
        /// <response code="404">project invoice not found</response>
        /// <response code="204">project invoice approved successfully</response>
        /// 
        [HttpPut("approve/{id:int}")]
        public async Task<IActionResult> Approve(int id)
        {
            //Get project invoice with its items from DB by its id
            var projectInvoice = await _service.GetProjectInvoiceWithItemsByIdAsync(id);
            if (projectInvoice == null)
            {
                return NotFound();
            }

            //Approve project invoice
            await _service.ApproveAsync(projectInvoice);

            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of suggested payments that are ready to be paid
        /// </summary>
        /// <returns>list of suggested payments</returns>
        /// <response code="200">returns list of suggested payments</response>
        /// 
        [HttpGet("paymentstopay")]
        public async Task<ActionResult<List<ProjectInvoicePaymentReadyToPayDto>?>> PaymentsToPay()
        {
            //Get a list of suggested payments that are ready to be paid
            var payments = await _queries.GetProjectInvoicePaymentReadyToPayAsync();
            
            return Ok(payments);
        }

        /// <summary>
        /// Retrieves a list of suggested payments by a list of suggested payments Ids
        /// </summary>
        /// <param name="paymentsIds">a list of suggested payments ids</param>
        /// <returns>a list of suggested payments</returns>
        /// <response code="200">returns list of suggested payments</response>
        /// 
        [HttpPost("PaymentsToPayByIds")]
        public async Task<ActionResult<List<ProjectInvoicePaymentReadyToPayDto>?>> PaymentsToPayByIds(
            [FromBody] List<int> paymentsIds)
        {
            //Get list of suggested payments from DB by a list of suggested payments ids
            var payments = await _queries.GetProjectInvoicePaymentToPayByIdsAsync(paymentsIds);
            
            return Ok(payments);
        }

        /// <summary>
        /// Pay a project invoice suggested payment and a add relevant bank checks and cash info
        /// </summary>
        /// <param name="paymentDto">suggested payment and bank checks and cash info</param>
        /// <returns>no content if suggested payment is paid successfully</returns>
        /// <response code="404">project invoice suggested payment not found</response>
        /// <response code="400">payment info are not valid</response>
        /// <response code="204">suggested payment is paid successfully</response>
        /// 
        [HttpPost("Payment")]
        public async Task<IActionResult> Payment([FromBody] ProjectInvoicePaymentCreationDto paymentDto)
        {
            //Get project invoice suggested payment from DB by its id
            var payment = await _service.GetProjectInvoicePaymentByIdAsync(paymentDto.PaymentId);

            if (payment == null)
            {
                return NotFound();
            }

            //Check if payment info are valid
            var isValid = _service.IsValidPayment(payment, paymentDto.CashList, paymentDto.ChecksList);
            
            if (!isValid)
            {
                return BadRequest("Payment amount should be equal to paid amount");
            }

            //Pay payment 
            await _service.PayPaymentAsync(payment, paymentDto.CashList, paymentDto.ChecksList);
            
            return NoContent();
        }

        /// <summary>
        /// Pay a list of project invoice suggested payment and a add relevant bank checks and cash info
        /// </summary>
        /// <param name="paymentDto">list of suggested payments and bank checks and cash info</param>
        /// <returns>no content if suggested payments are paid successfully</returns>
        /// <response code="404">list of project invoice suggested payments not found</response>
        /// <response code="400">list of payments info are not valid</response>
        /// <response code="204">list of suggested payments are paid successfully</response>
        /// 
        [HttpPost("paymentgroup")]
        public async Task<IActionResult> PaymentGroup([FromBody] ProjectInvoiceGroupPaymentCreationDto paymentDto)
        {
            //Get list of suggested payments by a list of suggested payments Ids
            var payments = await _service.GetProjectInvoicePaymentsByIdsAsync(paymentDto.PaymentIds);

            //Check if the retrieved list doesn't match dto info
            if (payments == null || payments.Count != paymentDto.PaymentIds.Count)
            {
                return NotFound();
            }

            //check if dto payments info are valid
            var isValid = _service.IsValidPaymentGroup(payments, paymentDto.CashList, paymentDto.ChecksList);
            if (!isValid)
            {
                return BadRequest("Payment amount should be equal to paid amount");
            }

            //pay the list of payments
            var done = await _service.PayPaymentGroupAsync(payments, paymentDto.CashList, paymentDto.ChecksList);
            if (!done)
            {
                return BadRequest("An error occured. Payment couldn't be paid");
            }

            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of projects as a list of keyvalue pairs
        /// </summary>
        /// <returns>a list of projects as a list of keyvalue pairs</returns>
        /// <response code="200">a list of projects as a list of keyvalue pairs</response>
        ///
        [HttpGet("projectskeyvalue")]
        public async Task<ActionResult<List<KeyValueDto>>> GetProjectsKeyValue()
        {
            // Get a list of projects from DB as a list of keyvalue pairs
            var projects = await _queries.GetProjectsKeyValueAsync();

            return Ok(projects);
        }

        /// <summary>
        /// Retrieves a list of suppliers as a list of keyvalue pairs
        /// </summary>
        /// <returns>a list of suppliers as a list of keyvalue pairs</returns>
        /// <response code="200">a list of suppliers as a list of keyvalue pairs</response>
        ///
        [HttpGet("supplierskeyvalue")]
        public async Task<ActionResult<List<KeyValueDto>>> GetSuppliersKeyValue()
        {
            // Get a list of suppliers from DB as a list of keyvalue pairs
            var suppliers = await _queries.GetSuppliersKeyValueAsync();
            
            return Ok(suppliers);
        }

        /// <summary>
        /// Retrieves a list of expenses items as a list of keyvalue pairs
        /// </summary>
        /// <returns>a list of expenses items as a list of keyvalue pairs</returns>
        /// <response code="200">a list of expenses items as a list of keyvalue pairs</response>
        ///
        [HttpGet("itemskeyvalue")]
        public async Task<ActionResult<List<KeyValueDto>>> GetItemsKeyValue()
        {
            // Get a list of expenses items from DB as a list of keyvalue pairs
            var items = await _queries.GetItemsKeyValueAsync();
            
            return Ok(items);
        }

        /// <summary>
        /// Retrieves a list of bank accounts as a list of keyvalue pairs
        /// </summary>
        /// <returns>a list of bank accounts as a list of keyvalue pairs</returns>
        /// <response code="200">a list of bank accounts as a list of keyvalue pairs</response>
        ///
        [HttpGet("bankaccountskeyvalue")]
        public async Task<ActionResult<List<KeyValueDto>>> GetBankAccountsKeyValue()
        {
            // Get a list of bank accounts from DB as a list of keyvalue pairs
            var bankAccounts = await _queries.GetBankAccountsKeyValue();
            
            return Ok(bankAccounts);
        }

        /// <summary>
        /// Retrieves list of project invoices detailed info by paging and searching criteria
        /// </summary>
        /// <param name="requestDto">paging and searching criteria</param>
        /// <returns>list of project invoices detailed info</returns>
        /// <response code="200">returns a list of project invoices detailed info</response>
        ///
        [HttpPost("projectinvoiceview")]
        public async Task<ActionResult<ProjectInvoiceViewResponseDto>> GetProjectInvoiceView
            ([FromBody] ProjectInvoiceViewRequestDto requestDto)
        {
            ProjectInvoiceState? state = requestDto.State == null ? null : (ProjectInvoiceState)requestDto.State;
            
            //Get number of results rows from DB for pagination  
            var count = await _queries.GetProjectInvoiceViewCountAsync(requestDto.Id, requestDto.Reference, requestDto.ProjectId,
                requestDto.SupplierId, state, requestDto.FromDate, requestDto.ToDate);

            // Get list of project invoices detailed info
            var data = await _queries.GetProjectInvoiceViewAsync(requestDto.Page, requestDto.PageSize,requestDto.Id, 
                requestDto.Reference, requestDto.ProjectId, requestDto.SupplierId, state, requestDto.FromDate, 
                requestDto.ToDate);
            
            var result = new ProjectInvoiceViewResponseDto { Data = data, TotalRecords = count };

            return Ok(result);
        }
    }
}
