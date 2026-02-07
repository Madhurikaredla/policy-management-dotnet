using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyManagement.Constants;
using PolicyManagement.DTOs;
using PolicyManagement.Filters;
using PolicyManagement.Services;

namespace PolicyManagement.Controllers;

[ApiController]
[Route("api/policies")]

public class PolicyController : ControllerBase
{
    private readonly IPolicyService policyService;
    private readonly ILogger<PolicyController> logger;

    public PolicyController(IPolicyService policyServiceObject, ILogger<PolicyController> logger)
    {
        this.policyService = policyServiceObject;
        this.logger = logger;
    }

    /// <summary>
    /// Get all active policies (accessible to all users)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult>GetPolicies()
    {
        try
        {
            logger.LogInformation("Fetching all policies");
            var policies = await this.policyService.GetAllPoliciesAsync();
            logger.LogInformation("Retrieved {Count} policies", policies.Count());
            return Ok(new { message = "Policies retrieved successfully", data = policies });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching all policies");
            return StatusCode(500, new { message = "An error occurred while retrieving policies", error = ex.Message });
        }
    }

    [HttpGet("{id}", Name = "GetPolicyById")]
    public async Task<IActionResult> GetPolicyById(int id)
    {
        try
        {
            logger.LogInformation("Fetching policy with ID: {PolicyId}", id);
            var policy = await this.policyService.GetPolicyByIdAsync(id);
            
            if (policy is null)
            {
                logger.LogWarning("Policy with ID {PolicyId} not found", id);
                return NotFound(new { message = $"Policy with ID {id} not found" });
            }
            
            logger.LogInformation("Policy with ID {PolicyId} retrieved successfully", id);
            return Ok(new { message = "Policy retrieved successfully", data = policy });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching policy with ID: {PolicyId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving policy", error = ex.Message });
        }
    }

    [HttpGet("search", Name = "SearchPolicies")]
    public async Task<IActionResult> SearchPolicies([FromQuery] decimal? minAmount, [FromQuery] decimal? maxAmount)
    {
        try
        {
            logger.LogInformation("Searching policies with premium between {MinAmount} and {MaxAmount}", minAmount, maxAmount);
    
            var policies = await this.policyService.SearchPoliciesByAmountAsync(minAmount, maxAmount);

            if (policies is null || !policies.Any())
            {
                logger.LogWarning("No policies found in the specified amount range {MinAmount} to {MaxAmount}", minAmount, maxAmount);
                return NotFound(new { message = $"No policies found in the specified amount range ${minAmount} to ${maxAmount}" });
            }

            logger.LogInformation("Found {Count} policies in the specified amount range", policies.Count());
            return Ok(new { message = "Policies retrieved successfully", data = policies });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while searching policies with amount range {MinAmount} to {MaxAmount}", minAmount, maxAmount);
            return StatusCode(500, new { message = "An error occurred while searching policies", error = ex.Message });
        }
    }


    [HttpGet("status", Name = "GetPoliciesByStatus")]
    public async Task<IActionResult> GetPoliciesByStatus ([FromQuery] bool isActive)
    {
        try
        {
            logger.LogInformation("Fetching policies by status: {Status}", isActive ? "active" : "inactive");
            var policies = await this.policyService.GetPoliciesByStatusAsync(isActive);

            if (policies is null || !policies.Any())
            {
                logger.LogWarning("No policies found with status {Status}", isActive ? "active" : "inactive");
                return NotFound(new { message = $"No policies found with status {(isActive ? "active" : "inactive")}" });
            }

            logger.LogInformation("Found {Count} policies with status {Status}", policies.Count(), isActive ? "active" : "inactive");
            return Ok(new { message = "Policies retrieved successfully", data = policies });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching policies by status: {Status}", isActive ? "active" : "inactive");
            return StatusCode(500, new { message = "An error occurred while retrieving policies", error = ex.Message });
        }
    }

    // [HttpGet("status/{status}", Name = "GetPoliciesByStatusFromRoute")]
    // public async Task<IActionResult> GetPoliciesByStatusFromRoute ([FromRoute] string status)
    // {
    //     return Ok(new {message = $"Policies with status {status} retrieved successfully"});
    // }

    /// <summary>
    /// Admin creates a new policy
    /// </summary>
    [HttpPost(Name = "CreatePolicy")]
    [Authorize(Roles = Roles.ROLE_ADMIN)]
    public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyDto policy)
    {
        try
        {
            logger.LogInformation("Attempting to create a new policy with code: {PolicyCode}", policy.PolicyCode);
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Policy creation failed due to validation errors");
                return BadRequest(new { message = "Validation failed", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var createdPolicy = await this.policyService.CreatePolicyAsync(policy);
            logger.LogInformation("Policy created successfully with ID: {PolicyId}", createdPolicy.Id);
            return CreatedAtRoute("GetPolicyById", new { id = createdPolicy.Id }, new { message = "Policy created successfully", data = createdPolicy });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating policy with code: {PolicyCode}", policy.PolicyCode);
            return StatusCode(500, new { message = "An error occurred while creating policy", error = ex.Message });
        }
    }

    /// <summary>
    /// Admin updates a policy
    /// </summary>
    [HttpPut("{id}", Name = "UpdatePolicy")]
    [Authorize(Roles = Roles.ROLE_ADMIN)]
    public async Task<IActionResult> UpdatePolicy(int id, [FromBody] UpdatePolicyDto policy)
    {
        try
        {
            logger.LogInformation("Attempting to update policy with ID: {PolicyId}", id);
            var updatedPolicy = await this.policyService.UpdatePolicyAsync(id, policy);
            if (updatedPolicy is null)
            {
                logger.LogWarning("Policy with ID {PolicyId} not found for update", id);
                return NotFound(new { message = $"Policy with ID {id} not found" });
            }
            logger.LogInformation("Policy with ID {PolicyId} updated successfully", id);
            return Ok(new { message = "Policy updated successfully", data = updatedPolicy });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating policy with ID: {PolicyId}", id);
            return StatusCode(500, new { message = "An error occurred while updating policy", error = ex.Message });
        }
    }

    /// <summary>
    /// Admin activates or deactivates a policy
    /// </summary>
    [HttpPatch("{id}/status", Name = "UpdatePolicyStatus")]
    [Authorize(Roles = Roles.ROLE_ADMIN)]
    public async Task<IActionResult> UpdatePolicyStatus(int id, [FromBody] UpdatePolicyStatusDto statusUpdate)
    {
        try
        {
            logger.LogInformation("Attempting to update status for policy with ID: {PolicyId}", id);
            // if (!ModelState.IsValid)
            // {
            //     logger.LogWarning("Status update failed for policy {PolicyId}: IsActive field is required", id);
            //     return BadRequest(new { message = "IsActive field is required" });
            // }

            var updatedPolicy = await this.policyService.UpdatePolicyStatusAsync(id, statusUpdate);
            if (updatedPolicy is null)
            {
                logger.LogWarning("Policy with ID {PolicyId} not found for status update", id);
                return NotFound(new { message = $"Policy with ID {id} not found" });
            }
            logger.LogInformation("Policy status updated successfully for policy ID: {PolicyId}", id);
            return Ok(new { message = "Policy status updated successfully", data = updatedPolicy });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating status for policy with ID: {PolicyId}", id);
            return StatusCode(500, new { message = "An error occurred while updating policy status", error = ex.Message });
        }
    }

    /// <summary>
    /// Admin deletes a policy
    /// </summary>
    [HttpDelete("{id}", Name = "DeletePolicy")]
    [Authorize(Roles = Roles.ROLE_ADMIN)]
    public async Task<IActionResult> DeletePolicy(int id)
    {
        try
        {
            logger.LogInformation("Attempting to delete policy with ID: {PolicyId}", id);
            var deletedPolicy = await this.policyService.DeletePolicyAsync(id);
            if (deletedPolicy is null)
            {
                logger.LogWarning("Policy with ID {PolicyId} not found for deletion", id);
                return NotFound(new { message = $"Policy with ID {id} not found" });
            }
            logger.LogInformation("Policy with ID {PolicyId} deleted successfully", id);
            return Ok(new { message = "Policy deleted successfully", data = deletedPolicy });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting policy with ID: {PolicyId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting policy", error = ex.Message });
        }
    }
}