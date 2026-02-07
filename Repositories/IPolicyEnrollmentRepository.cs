using PolicyManagement.Models;

namespace PolicyManagement.Repositories;

public interface IPolicyEnrollmentRepository
{
    Task<PolicyEnrollment> CreateEnrollmentAsync(PolicyEnrollment enrollment);
    Task<PolicyEnrollment?> GetEnrollmentByIdAsync(int id);
    Task<IEnumerable<PolicyEnrollment>> GetEnrollmentsByUserIdAsync(int userId);
    Task<IEnumerable<PolicyEnrollment>> GetEnrollmentsByStatusAsync(string status);
    Task<IEnumerable<PolicyEnrollment>> GetAllEnrollmentsAsync();
    Task<PolicyEnrollment?> GetExistingEnrollmentAsync(int userId, int policyId);
    Task<PolicyEnrollment> UpdateEnrollmentAsync(PolicyEnrollment enrollment);
}
