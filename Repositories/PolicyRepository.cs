using Microsoft.EntityFrameworkCore;
using PolicyManagement.Data;
using PolicyManagement.DTOs;
using PolicyManagement.Models;

namespace PolicyManagement.Repositories;

public class PolicyRepository : IPolicyRepository
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<PolicyRepository> logger;

    public PolicyRepository(ApplicationDbContext contextObject, ILogger<PolicyRepository> logger) 
    {
        this.context = contextObject;
        this.logger = logger;
    }

    public async Task<IEnumerable<Policy>> GetAllPoliciesAsync()
    {
        return await this.context.Policies
            .Where(p => p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Policy?> GetPolicyByIdAsync(int id)
    {
        return await this.context.Policies
        .Where(p => p.Id == id && p.DeletedAt == null)
        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Policy>?> SearchPoliciesByAmountAsync(decimal? minAmount, decimal? maxAmount)
    {
        var query = this.context.Policies.AsQueryable();
        if (minAmount.HasValue)
        {
            query = query.Where(p => p.PremiumAmount >= minAmount.Value);
        }
        if (maxAmount.HasValue)
        {
            query = query.Where(p => p.PremiumAmount <= maxAmount.Value);
        }
        query = query.Where(p => p.DeletedAt == null);
        return await query.ToListAsync();
    }


     public async Task<IEnumerable<Policy>?> GetPoliciesByStatusAsync(bool isActive)
    {
        return await this.context.Policies
            .Where(p => p.IsActive == isActive && p.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<Policy> CreatePolicyAsync(CreatePolicyDto policy)
    {
        logger.LogInformation("Creating policy in database with code: {PolicyCode}", policy.PolicyCode);
        var utcNow = DateTime.UtcNow;
        var newPolicy = new Policy
        {
            PolicyCode = policy.PolicyCode,
            PolicyName = policy.PolicyName,
            Description = policy.Description,
            PremiumAmount = policy.PremiumAmount,
            IsActive = policy.IsActive,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
        this.context.Policies.Add(newPolicy);
        await this.context.SaveChangesAsync();
        logger.LogInformation("Policy created in database with ID: {PolicyId}", newPolicy.Id);
        return newPolicy;
    }

    public async Task<Policy?> UpdatePolicyAsync(int id, UpdatePolicyDto policy)
    {
        var existingPolicy = await this.context.Policies.Where(p => p.Id == id && p.DeletedAt == null).FirstOrDefaultAsync();

        if (existingPolicy == null)
        {
            return null;
        }

        // Update fields only which are provided in the DTO
        if (!string.IsNullOrEmpty(policy.PolicyName))
        {
            existingPolicy.PolicyName = policy.PolicyName;
        }
        if (!string.IsNullOrEmpty(policy.Description))
        {
            existingPolicy.Description = policy.Description;
        }
        if (policy.PremiumAmount.HasValue)
        {
            existingPolicy.PremiumAmount = policy.PremiumAmount.Value;
        }
        if (policy.IsActive.HasValue)
        {
            existingPolicy.IsActive = policy.IsActive.Value;
        }
        existingPolicy.UpdatedAt = DateTime.UtcNow;
        // Ensure CreatedAt is always UTC
        if (existingPolicy.CreatedAt.Kind != DateTimeKind.Utc)
        {
            existingPolicy.CreatedAt = DateTime.SpecifyKind(existingPolicy.CreatedAt, DateTimeKind.Utc);
        }

        this.context.Policies.Update(existingPolicy);
        await this.context.SaveChangesAsync();

        return existingPolicy;
    }

    public async Task<Policy?> DeletePolicyAsync(int id)
    {
        var existingPolicy = await this.context.Policies.Where(p => p.Id == id && p.DeletedAt == null).FirstOrDefaultAsync();
        if (existingPolicy == null)
        {
            return null;
        }

        // soft delete by setting deleted flag
        existingPolicy.DeletedAt = DateTime.UtcNow;
        existingPolicy.IsActive = false; // Optionally set IsActive to false when deleted
        // Ensure all DateTime fields are UTC
        if (existingPolicy.CreatedAt.Kind != DateTimeKind.Utc)
        {
            existingPolicy.CreatedAt = DateTime.SpecifyKind(existingPolicy.CreatedAt, DateTimeKind.Utc);
        }
        if (existingPolicy.UpdatedAt.HasValue && existingPolicy.UpdatedAt.Value.Kind != DateTimeKind.Utc)
        {
            existingPolicy.UpdatedAt = DateTime.SpecifyKind(existingPolicy.UpdatedAt.Value, DateTimeKind.Utc);
        }
        if (existingPolicy.DeletedAt.HasValue && existingPolicy.DeletedAt.Value.Kind != DateTimeKind.Utc)
        {
            existingPolicy.DeletedAt = DateTime.SpecifyKind(existingPolicy.DeletedAt.Value, DateTimeKind.Utc);
        }
        this.context.Policies.Update(existingPolicy);
        await this.context.SaveChangesAsync();

        return existingPolicy;
    }

    public async Task<Policy?> GetPolicyByCodeAsync(string policyCode)
    {
        return await this.context.Policies
            .Where(p => p.PolicyCode == policyCode && p.DeletedAt == null)
            .FirstOrDefaultAsync();
    }
}