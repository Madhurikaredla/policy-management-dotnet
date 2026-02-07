using Microsoft.EntityFrameworkCore;
using PolicyManagement.Data;
using PolicyManagement.Models;

namespace PolicyManagement.Repositories;

public class PolicyEnrollmentRepository : IPolicyEnrollmentRepository
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<PolicyEnrollmentRepository> logger;

    public PolicyEnrollmentRepository(ApplicationDbContext contextobj, ILogger<PolicyEnrollmentRepository> logger)
    {
        this.context = contextobj;
        this.logger = logger;
    }

    public async Task<PolicyEnrollment> CreateEnrollmentAsync(PolicyEnrollment enrollment)
    {
        context.PolicyEnrollments.Add(enrollment);
        await context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<PolicyEnrollment?> GetEnrollmentByIdAsync(int id)
    {
        return await context.PolicyEnrollments
            .Include(e => e.User)
            .Include(e => e.Policy)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<PolicyEnrollment>> GetEnrollmentsByUserIdAsync(int userId)
    {
        return await context.PolicyEnrollments
            .Include(e => e.Policy)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.RequestedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PolicyEnrollment>> GetEnrollmentsByStatusAsync(string status)
    {
        return await context.PolicyEnrollments
            .Include(e => e.User)
            .Include(e => e.Policy)
            .Where(e => e.Status == status)
            .OrderByDescending(e => e.RequestedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PolicyEnrollment>> GetAllEnrollmentsAsync()
    {
        return await context.PolicyEnrollments
            .Include(e => e.User)
            .Include(e => e.Policy)
            .OrderByDescending(e => e.RequestedAt)
            .ToListAsync();
    }

    public async Task<PolicyEnrollment?> GetExistingEnrollmentAsync(int userId, int policyId)
    {
        return await context.PolicyEnrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.PolicyId == policyId);
    }

    public async Task<PolicyEnrollment> UpdateEnrollmentAsync(PolicyEnrollment enrollment)
    {
        // Ensure all DateTime fields are UTC
        if (enrollment.RequestedAt.Kind != DateTimeKind.Utc)
            enrollment.RequestedAt = DateTime.SpecifyKind(enrollment.RequestedAt, DateTimeKind.Utc);
        if (enrollment.ApprovedAt.HasValue && enrollment.ApprovedAt.Value.Kind != DateTimeKind.Utc)
            enrollment.ApprovedAt = DateTime.SpecifyKind(enrollment.ApprovedAt.Value, DateTimeKind.Utc);
        if (enrollment.RejectedAt.HasValue && enrollment.RejectedAt.Value.Kind != DateTimeKind.Utc)
            enrollment.RejectedAt = DateTime.SpecifyKind(enrollment.RejectedAt.Value, DateTimeKind.Utc);

        context.PolicyEnrollments.Update(enrollment);
        await context.SaveChangesAsync();
        return enrollment;
    }
}
