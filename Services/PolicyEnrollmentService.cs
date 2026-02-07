using PolicyManagement.DTOs;
using PolicyManagement.Models;
using PolicyManagement.Repositories;

namespace PolicyManagement.Services;

public class PolicyEnrollmentService : IPolicyEnrollmentService
{
    private readonly IPolicyEnrollmentRepository enrollmentRepository;
    private readonly IPolicyRepository policyRepository;
    private readonly ILogger<PolicyEnrollmentService> logger;

    public PolicyEnrollmentService(
        IPolicyEnrollmentRepository enrollmentRepository,
        IPolicyRepository policyRepository,
        ILogger<PolicyEnrollmentService> logger)
    {
        this.enrollmentRepository = enrollmentRepository;
        this.policyRepository = policyRepository;
        this.logger = logger;
    }

    public async Task<EnrollmentResponseDto> RequestEnrollmentAsync(int userId, EnrollmentRequestDto request)
    {
        logger.LogInformation("User {UserId} requesting enrollment for policy {PolicyId}", userId, request.PolicyId);

        // Check if policy exists and is active
        var policy = await policyRepository.GetPolicyByIdAsync(request.PolicyId);
        if (policy == null)
        {
            logger.LogWarning("Policy {PolicyId} not found", request.PolicyId);
            throw new InvalidOperationException("Policy not found");
        }

        if (!policy.IsActive)
        {
            logger.LogWarning("Policy {PolicyId} is not active", request.PolicyId);
            throw new InvalidOperationException("Policy is not active");
        }

        // Check if user already enrolled in this policy
        var existingEnrollment = await enrollmentRepository.GetExistingEnrollmentAsync(userId, request.PolicyId);
        if (existingEnrollment != null)
        {
            logger.LogWarning("User {UserId} already enrolled in policy {PolicyId}", userId, request.PolicyId);
            throw new InvalidOperationException("You have already requested enrollment for this policy");
        }

        // Create enrollment
        var enrollment = new PolicyEnrollment
        {
            UserId = userId,
            PolicyId = request.PolicyId,
            Status = "Pending",
            RequestedAt = DateTime.UtcNow
        };

        var createdEnrollment = await enrollmentRepository.CreateEnrollmentAsync(enrollment);
        logger.LogInformation("Enrollment {EnrollmentId} created for user {UserId}", createdEnrollment.Id, userId);

        // Load related data
        var enrollmentWithDetails = await enrollmentRepository.GetEnrollmentByIdAsync(createdEnrollment.Id);
        return MapToDto(enrollmentWithDetails!);
    }

    public async Task<IEnumerable<EnrollmentResponseDto>> GetMyEnrollmentsAsync(int userId)
    {
        logger.LogInformation("Fetching enrollments for user {UserId}", userId);
        var enrollments = await enrollmentRepository.GetEnrollmentsByUserIdAsync(userId);
        return enrollments.Select(MapToDto);
    }

    public async Task<IEnumerable<EnrollmentResponseDto>> GetEnrollmentsByStatusAsync(string status)
    {
        logger.LogInformation("Fetching enrollments with status {Status}", status);
        var enrollments = await enrollmentRepository.GetEnrollmentsByStatusAsync(status);
        return enrollments.Select(MapToDto);
    }

    public async Task<IEnumerable<EnrollmentResponseDto>> GetAllEnrollmentsAsync()
    {
        logger.LogInformation("Fetching all enrollments");
        var enrollments = await enrollmentRepository.GetAllEnrollmentsAsync();
        return enrollments.Select(MapToDto);
    }

    public async Task<EnrollmentResponseDto?> ApproveEnrollmentAsync(int enrollmentId, EnrollmentApprovalDto? approval)
    {
        logger.LogInformation("Approving enrollment {EnrollmentId}", enrollmentId);
        
        var enrollment = await enrollmentRepository.GetEnrollmentByIdAsync(enrollmentId);
        if (enrollment == null)
        {
            logger.LogWarning("Enrollment {EnrollmentId} not found", enrollmentId);
            return null;
        }

        if (enrollment.Status != "Pending")
        {
            logger.LogWarning("Enrollment {EnrollmentId} is not pending", enrollmentId);
            throw new InvalidOperationException("Only pending enrollments can be approved");
        }

        enrollment.Status = "Approved";
        enrollment.ApprovedAt = DateTime.UtcNow;
        enrollment.AdminRemarks = approval?.AdminRemarks;

        var updatedEnrollment = await enrollmentRepository.UpdateEnrollmentAsync(enrollment);
        logger.LogInformation("Enrollment {EnrollmentId} approved", enrollmentId);
        
        return MapToDto(updatedEnrollment);
    }

    public async Task<EnrollmentResponseDto?> RejectEnrollmentAsync(int enrollmentId, EnrollmentApprovalDto? approval)
    {
        logger.LogInformation("Rejecting enrollment {EnrollmentId}", enrollmentId);
        
        var enrollment = await enrollmentRepository.GetEnrollmentByIdAsync(enrollmentId);
        if (enrollment == null)
        {
            logger.LogWarning("Enrollment {EnrollmentId} not found", enrollmentId);
            return null;
        }

        if (enrollment.Status != "Pending")
        {
            logger.LogWarning("Enrollment {EnrollmentId} is not pending", enrollmentId);
            throw new InvalidOperationException("Only pending enrollments can be rejected");
        }

        enrollment.Status = "Rejected";
        enrollment.RejectedAt = DateTime.UtcNow;
        enrollment.AdminRemarks = approval?.AdminRemarks;

        var updatedEnrollment = await enrollmentRepository.UpdateEnrollmentAsync(enrollment);
        logger.LogInformation("Enrollment {EnrollmentId} rejected", enrollmentId);
        
        return MapToDto(updatedEnrollment);
    }

    private EnrollmentResponseDto MapToDto(PolicyEnrollment enrollment)
    {
        return new EnrollmentResponseDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            UserName = enrollment.User?.Name ?? string.Empty,
            UserEmail = enrollment.User?.Email ?? string.Empty,
            PolicyId = enrollment.PolicyId,
            PolicyName = enrollment.Policy?.PolicyName ?? string.Empty,
            PolicyCode = enrollment.Policy?.PolicyCode ?? string.Empty,
            PremiumAmount = enrollment.Policy?.PremiumAmount ?? 0,
            Status = enrollment.Status,
            RequestedAt = enrollment.RequestedAt,
            ApprovedAt = enrollment.ApprovedAt,
            RejectedAt = enrollment.RejectedAt,
            AdminRemarks = enrollment.AdminRemarks
        };
    }
}
