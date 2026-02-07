using PolicyManagement.DTOs;
using PolicyManagement.Models;
using PolicyManagement.Repositories;

namespace PolicyManagement.Services;

public class PolicyService : IPolicyService
{
    private readonly IPolicyRepository policyRepository;
    private readonly ILogger<PolicyService> logger;

    public PolicyService(IPolicyRepository policyRepositoryObject, ILogger<PolicyService> logger) 
    {
        this.policyRepository = policyRepositoryObject;
        this.logger = logger;
    }

    
    public async Task<IEnumerable<Policy>> GetAllPoliciesAsync()
    {
        return await this.policyRepository.GetAllPoliciesAsync();
    }

    public async Task<IEnumerable<Policy>?> GetPoliciesByStatusAsync(bool isActive)
    {
        return await this.policyRepository.GetPoliciesByStatusAsync(isActive);
    }

    public async Task<Policy?> GetPolicyByIdAsync(int id)
    {
        return await this.policyRepository.GetPolicyByIdAsync(id);
    }

    public async Task<IEnumerable<Policy>?> SearchPoliciesByAmountAsync(decimal? minAmount, decimal? maxAmount)
    {
        return await this.policyRepository.SearchPoliciesByAmountAsync(minAmount, maxAmount);
    }

    public async Task<Policy> CreatePolicyAsync(CreatePolicyDto policy)
    {
        logger.LogInformation("Creating new policy with code: {PolicyCode}", policy.PolicyCode);
        // Check for duplicate policy code
        var existingPolicy = await this.policyRepository.GetPolicyByCodeAsync(policy.PolicyCode);
        if (existingPolicy != null)
        {
            logger.LogWarning("Policy creation failed: Policy code {PolicyCode} already exists", policy.PolicyCode);
            throw new InvalidOperationException("Policy with this code already exists");
        }
        var createdPolicy = await this.policyRepository.CreatePolicyAsync(policy);
        logger.LogInformation("Policy created successfully with ID: {PolicyId}", createdPolicy.Id);
        return createdPolicy;
    }

    public async Task<Policy?> UpdatePolicyAsync(int id, UpdatePolicyDto policy)
    {
        var existingPolicy = await this.policyRepository.GetPolicyByIdAsync(id);
        if (existingPolicy == null)
        {
            return null;
        }

        return await this.policyRepository.UpdatePolicyAsync(id,policy);
    }

    public async Task<Policy?> DeletePolicyAsync(int id)
    {
        logger.LogInformation("Attempting to delete policy with ID: {PolicyId}", id);
        var existingPolicy = await this.policyRepository.GetPolicyByIdAsync(id);
        if (existingPolicy == null)
        {
            logger.LogWarning("Policy with ID {PolicyId} not found for deletion", id);
            return null;
        }

        var deletedPolicy = await this.policyRepository.DeletePolicyAsync(id);
        logger.LogInformation("Policy with ID {PolicyId} deleted successfully", id);
        return deletedPolicy;
    }

    public async Task<Policy?> UpdatePolicyStatusAsync(int id, UpdatePolicyStatusDto statusUpdate)
    {
        var existingPolicy = await this.policyRepository.GetPolicyByIdAsync(id);
        if (existingPolicy == null)
        {
            return null;
        }

        existingPolicy.IsActive = statusUpdate.IsActive;
        existingPolicy.UpdatedAt = DateTime.UtcNow;

        return await this.policyRepository.UpdatePolicyAsync(id, new UpdatePolicyDto
        {
            PolicyName = existingPolicy.PolicyName,
            Description = existingPolicy.Description,
            PremiumAmount = existingPolicy.PremiumAmount,
            IsActive = existingPolicy.IsActive
        });
    }
}