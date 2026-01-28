using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.Workflows;
using MedicSoft.Application.Services.Workflows;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    [ApiController]
    [Route("api/system-admin/workflows")]
    [Authorize(Roles = "SystemAdmin")]
    public class WorkflowController : ControllerBase
    {
        private readonly MedicSoftDbContext _context;
        private readonly IWorkflowEngine _workflowEngine;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(
            MedicSoftDbContext context,
            IWorkflowEngine workflowEngine,
            ILogger<WorkflowController> logger)
        {
            _context = context;
            _workflowEngine = workflowEngine;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<WorkflowDto>>> GetAll()
        {
            var workflows = await _context.Workflows
                .Include(w => w.Actions.OrderBy(a => a.Order))
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            var dtos = workflows.Select(w => MapToDto(w)).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkflowDto>> GetById(int id)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Actions.OrderBy(a => a.Order))
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workflow == null)
                return NotFound();

            return Ok(MapToDto(workflow));
        }

        [HttpPost]
        public async Task<ActionResult<WorkflowDto>> Create([FromBody] CreateWorkflowDto dto)
        {
            var workflow = new Workflow
            {
                Name = dto.Name,
                Description = dto.Description,
                IsEnabled = dto.IsEnabled,
                TriggerType = dto.TriggerType,
                TriggerConfig = dto.TriggerConfig,
                StopOnError = dto.StopOnError,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System",
                Actions = new List<WorkflowAction>()
            };

            if (dto.Actions != null)
            {
                foreach (var actionDto in dto.Actions)
                {
                    workflow.Actions.Add(new WorkflowAction
                    {
                        Order = actionDto.Order,
                        ActionType = actionDto.ActionType,
                        Config = actionDto.Config,
                        Condition = actionDto.Condition,
                        DelaySeconds = actionDto.DelaySeconds,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            _context.Workflows.Add(workflow);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Workflow created: {workflow.Id} - {workflow.Name}");

            return CreatedAtAction(nameof(GetById), new { id = workflow.Id }, MapToDto(workflow));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WorkflowDto>> Update(int id, [FromBody] UpdateWorkflowDto dto)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Actions)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workflow == null)
                return NotFound();

            workflow.Name = dto.Name;
            workflow.Description = dto.Description;
            workflow.IsEnabled = dto.IsEnabled;
            workflow.TriggerType = dto.TriggerType;
            workflow.TriggerConfig = dto.TriggerConfig;
            workflow.StopOnError = dto.StopOnError;
            workflow.UpdatedAt = DateTime.UtcNow;
            workflow.UpdatedBy = User.Identity?.Name ?? "System";

            // Update actions
            _context.WorkflowActions.RemoveRange(workflow.Actions);

            if (dto.Actions != null)
            {
                workflow.Actions = new List<WorkflowAction>();
                foreach (var actionDto in dto.Actions)
                {
                    workflow.Actions.Add(new WorkflowAction
                    {
                        WorkflowId = workflow.Id,
                        Order = actionDto.Order,
                        ActionType = actionDto.ActionType,
                        Config = actionDto.Config,
                        Condition = actionDto.Condition,
                        DelaySeconds = actionDto.DelaySeconds,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Workflow updated: {workflow.Id} - {workflow.Name}");

            return Ok(MapToDto(workflow));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var workflow = await _context.Workflows.FindAsync(id);

            if (workflow == null)
                return NotFound();

            _context.Workflows.Remove(workflow);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Workflow deleted: {id}");

            return NoContent();
        }

        [HttpPost("{id}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            var workflow = await _context.Workflows.FindAsync(id);

            if (workflow == null)
                return NotFound();

            workflow.IsEnabled = !workflow.IsEnabled;
            workflow.UpdatedAt = DateTime.UtcNow;
            workflow.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Workflow toggled: {id} - Enabled: {workflow.IsEnabled}");

            return Ok(new { isEnabled = workflow.IsEnabled });
        }

        [HttpPost("{id}/test")]
        public async Task<ActionResult<WorkflowExecution>> Test(int id, [FromBody] object testData)
        {
            var execution = await _workflowEngine.ExecuteWorkflowAsync(id, testData);

            if (execution == null)
                return NotFound("Workflow not found or not enabled");

            return Ok(execution);
        }

        [HttpGet("{id}/executions")]
        public async Task<ActionResult<List<WorkflowExecution>>> GetExecutions(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var executions = await _context.WorkflowExecutions
                .Where(e => e.WorkflowId == id)
                .Include(e => e.ActionExecutions)
                .OrderByDescending(e => e.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(executions);
        }

        [HttpGet("executions/{executionId}")]
        public async Task<ActionResult<WorkflowExecution>> GetExecution(int executionId)
        {
            var execution = await _context.WorkflowExecutions
                .Include(e => e.Workflow)
                .Include(e => e.ActionExecutions)
                    .ThenInclude(ae => ae.WorkflowAction)
                .FirstOrDefaultAsync(e => e.Id == executionId);

            if (execution == null)
                return NotFound();

            return Ok(execution);
        }

        private WorkflowDto MapToDto(Workflow workflow)
        {
            return new WorkflowDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                IsEnabled = workflow.IsEnabled,
                TriggerType = workflow.TriggerType,
                TriggerConfig = workflow.TriggerConfig,
                StopOnError = workflow.StopOnError,
                CreatedAt = workflow.CreatedAt,
                CreatedBy = workflow.CreatedBy,
                UpdatedAt = workflow.UpdatedAt,
                UpdatedBy = workflow.UpdatedBy,
                Actions = workflow.Actions?.Select(a => new WorkflowActionDto
                {
                    Id = a.Id,
                    Order = a.Order,
                    ActionType = a.ActionType,
                    Config = a.Config,
                    Condition = a.Condition,
                    DelaySeconds = a.DelaySeconds
                }).ToList()
            };
        }
    }
}
