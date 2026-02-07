using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyManagement.DTOs;
using PolicyManagement.Services;
using System.Security.Claims;

namespace PolicyManagement.Controllers;
using PolicyManagement.Constants;

/// <summary>
/// Handles policy enrollment operations for users and admins
/// </summary>
 [ApiController]
[Route("api/enrollments")]
public class EnrollmentController : ControllerBase
{
    private readonly IPolicyEnrollmentService enrollmentService;
    private readonly ILogger<EnrollmentController> logger;

    public EnrollmentController(IPolicyEnrollmentService enrollmentService, ILogger<EnrollmentController> logger)
    {
        this.enrollmentService = enrollmentService;
        this.logger = logger;
    }

    /// <summary>
    /// User requests to enroll in a policy
    /// </summary>
    [HttpPost("policy/{policyId}/enroll")]
    [Authorize(Roles = Roles.ROLE_USER + "," + Roles.ROLE_ADMIN)]
    public async Task<IActionResult> EnrollInPolicy(int policyId)
    {
        try
        {
            Console.WriteLine($"Received enrollment request for policy ID: {policyId} from user with claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            // Find the first NameIdentifier claim that is an integer (user ID)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            logger.LogInformation("User ID claim value: {UserIdClaim}, Email claim value: {EmailClaim}", userIdClaim, emailClaim);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                logger.LogWarning("Invalid user token");
                return Unauthorized(new { message = "Invalid user token" });
            }

            var request = new EnrollmentRequestDto { PolicyId = policyId };
            var enrollment = await enrollmentService.RequestEnrollmentAsync(userId, request);
            logger.LogInformation("Enrollment request submitted successfully for user {UserId} and policy {PolicyId}", userId, policyId);
            return Ok(new { message = "Enrollment request submitted successfully", data = enrollment });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Enrollment request failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing enrollment for policy {PolicyId}", policyId);
            return StatusCode(500, new { message = "An error occurred while processing enrollment", error = ex.Message });
        }
    }

    /// <summary>
    /// User views their own enrollment requests
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = Roles.ROLE_USER + "," + Roles.ROLE_ADMIN)]
    public async Task<IActionResult> GetMyEnrollments(int userId)
    {
        try
        {
            logger.LogInformation("Fetching enrollments for current user");            
            // var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;
            // if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            // {
            //     logger.LogWarning("Invalid user token for enrollment fetch");
            //     return Unauthorized(new { message = "Invalid user token" });
            // }

            var enrollments = await enrollmentService.GetMyEnrollmentsAsync(userId);
            logger.LogInformation("Found {Count} enrollments for user ID: {UserId}", enrollments.Count(), userId);
            return Ok(new { message = "Enrollments retrieved successfully", data = enrollments });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching enrollments for current user");
            return StatusCode(500, new { message = "An error occurred while retrieving enrollments", error = ex.Message });
        }
    }

    /// <summary>
    /// Admin views all enrollments or filter by status
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = Roles.ROLE_ADMIN)]
    public async Task<IActionResult> GetAllEnrollments([FromQuery] string? status)
    {
        try
        {
            logger.LogInformation("Admin fetching enrollments with status filter: {Status}", status ?? "all");

            IEnumerable<EnrollmentResponseDto> enrollments;
            
            if (!string.IsNullOrEmpty(status))
            {
                enrollments = await enrollmentService.GetEnrollmentsByStatusAsync(status);
            }
            else
            {
                enrollments = await enrollmentService.GetAllEnrollmentsAsync();
            }

            logger.LogInformation("Retrieved {Count} enrollments with status filter: {Status}", enrollments.Count(), status ?? "all");
            return Ok(new { message = "Enrollments retrieved successfully", data = enrollments });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching enrollments with status filter: {Status}", status ?? "all");
            return StatusCode(500, new { message = "An error occurred while retrieving enrollments", error = ex.Message });
        }
    }

    /// <summary>
    /// Admin approves an enrollment request
    /// </summary>
    [HttpPost("admin/{id}/approve")]
    [Authorize(Roles = Roles.ROLE_ADMIN)]
    public async Task<IActionResult> ApproveEnrollment(int id, [FromBody] EnrollmentApprovalDto? approval)
    {
        try
        {
            logger.LogInformation("Admin approving enrollment {EnrollmentId}", id);

            var enrollment = await enrollmentService.ApproveEnrollmentAsync(id, approval);
            
            if (enrollment == null)
            {
                logger.LogWarning("Enrollment with ID {EnrollmentId} not found for approval", id);
                return NotFound(new { message = $"Enrollment with ID {id} not found" });
            }

            logger.LogInformation("Enrollment {EnrollmentId} approved successfully", id);
            return Ok(new { message = "Enrollment approved successfully", data = enrollment });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while approving enrollment with ID: {EnrollmentId}", id);
            return StatusCode(500, new { message = "An error occurred while approving enrollment", error = ex.Message });
        }
    }

    /// <summary>
    /// Admin rejects an enrollment request
    /// </summary>
    [HttpPost("admin/{id}/reject")]
    [Authorize(Roles = Roles.ROLE_ADMIN)]
    public async Task<IActionResult> RejectEnrollment(int id, [FromBody] EnrollmentApprovalDto? approval)
    {
        try
        {
            logger.LogInformation("Admin rejecting enrollment {EnrollmentId}", id);

            var enrollment = await enrollmentService.RejectEnrollmentAsync(id, approval);
            
            if (enrollment == null)
            {
                logger.LogWarning("Enrollment with ID {EnrollmentId} not found for rejection", id);
                return NotFound(new { message = $"Enrollment with ID {id} not found" });
            }

            logger.LogInformation("Enrollment {EnrollmentId} rejected successfully", id);
            return Ok(new { message = "Enrollment rejected successfully", data = enrollment });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while rejecting enrollment with ID: {EnrollmentId}", id);
            return StatusCode(500, new { message = "An error occurred while rejecting enrollment", error = ex.Message });
        }
    }
}
