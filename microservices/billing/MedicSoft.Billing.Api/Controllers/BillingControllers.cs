using Microsoft.AspNetCore.Mvc;
using MedicSoft.Billing.Api.Models;
using MedicSoft.Billing.Api.Services;
using MedicSoft.Shared.Authentication;

namespace MedicSoft.Billing.Api.Controllers;

[Route("api/[controller]")]
public class SubscriptionsController : MicroserviceBaseController
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Get all subscription plans
    /// </summary>
    [HttpGet("plans")]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetPlans()
    {
        var plans = await _subscriptionService.GetAllPlansAsync();
        return Ok(plans);
    }

    /// <summary>
    /// Get clinic subscription
    /// </summary>
    [HttpGet("clinic/{clinicId}")]
    public async Task<ActionResult<ClinicSubscriptionDto>> GetClinicSubscription(Guid clinicId)
    {
        var subscription = await _subscriptionService.GetClinicSubscriptionAsync(clinicId, GetTenantId());

        if (subscription == null)
            return NotFound($"No subscription found for clinic {clinicId}");

        return Ok(subscription);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Subscriptions.Microservice" });
    }
}

[Route("api/[controller]")]
public class PaymentsController : MicroserviceBaseController
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Get all payments for a clinic
    /// </summary>
    [HttpGet("clinic/{clinicId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByClinic(Guid clinicId)
    {
        var payments = await _paymentService.GetPaymentsByClinicAsync(clinicId, GetTenantId());
        return Ok(payments);
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var clinicId = GetClinicId();
        if (clinicId == null)
            return BadRequest("Clinic ID is required");

        try
        {
            var payment = await _paymentService.CreatePaymentAsync(dto, clinicId.Value, GetTenantId());
            return CreatedAtAction(nameof(GetByClinic), new { clinicId = clinicId.Value }, payment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Payments.Microservice" });
    }
}

[Route("api/[controller]")]
public class ExpensesController : MicroserviceBaseController
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>
    /// Get all expenses for a clinic
    /// </summary>
    [HttpGet("clinic/{clinicId}")]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetByClinic(Guid clinicId)
    {
        var expenses = await _expenseService.GetExpensesByClinicAsync(clinicId, GetTenantId());
        return Ok(expenses);
    }

    /// <summary>
    /// Create a new expense
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create([FromBody] CreateExpenseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var clinicId = GetClinicId();
        if (clinicId == null)
            return BadRequest("Clinic ID is required");

        try
        {
            var expense = await _expenseService.CreateExpenseAsync(dto, clinicId.Value, GetTenantId());
            return CreatedAtAction(nameof(GetByClinic), new { clinicId = clinicId.Value }, expense);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Expenses.Microservice" });
    }
}
