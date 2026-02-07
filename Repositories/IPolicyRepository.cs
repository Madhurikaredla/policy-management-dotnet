using PolicyManagement.DTOs;
using PolicyManagement.Models;

namespace PolicyManagement.Repositories;

public interface IPolicyRepository
{
    Task<IEnumerable<Policy>> GetAllPoliciesAsync();

    Task<Policy?> GetPolicyByIdAsync(int id);

    Task<Policy?> GetPolicyByCodeAsync(string policyCode);

    Task<IEnumerable<Policy>?> SearchPoliciesByAmountAsync(decimal? minAmount, decimal? maxAmount);

    Task<IEnumerable<Policy>?> GetPoliciesByStatusAsync(bool isActive);
    
    Task<Policy> CreatePolicyAsync(CreatePolicyDto policy);

    Task<Policy?> UpdatePolicyAsync(int id, UpdatePolicyDto policy);

    Task<Policy?> DeletePolicyAsync(int id);
}