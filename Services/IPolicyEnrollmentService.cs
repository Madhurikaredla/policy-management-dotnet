using PolicyManagement.DTOs;

namespace PolicyManagement.Services;

public interface IPolicyEnrollmentService
{
    Task<EnrollmentResponseDto> RequestEnrollmentAsync(int userId, EnrollmentRequestDto request);
    Task<IEnumerable<EnrollmentResponseDto>> GetMyEnrollmentsAsync(int userId);
    Task<IEnumerable<EnrollmentResponseDto>> GetEnrollmentsByStatusAsync(string status);
    Task<IEnumerable<EnrollmentResponseDto>> GetAllEnrollmentsAsync();
    Task<EnrollmentResponseDto?> ApproveEnrollmentAsync(int enrollmentId, EnrollmentApprovalDto? approval);
    Task<EnrollmentResponseDto?> RejectEnrollmentAsync(int enrollmentId, EnrollmentApprovalDto? approval);
}
