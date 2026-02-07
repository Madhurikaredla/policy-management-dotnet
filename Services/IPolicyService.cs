using PolicyManagement.DTOs;
using PolicyManagement.Models;

namespace PolicyManagement.Services;

public interface IPolicyService
{
    Task<IEnumerable<Policy>> GetAllPoliciesAsync();

    Task<Policy?> GetPolicyByIdAsync(int id);

    Task<IEnumerable<Policy>?> SearchPoliciesByAmountAsync(decimal? minAmount, decimal? maxAmount);

    Task<IEnumerable<Policy>?> GetPoliciesByStatusAsync(bool isActive);

    Task<Policy> CreatePolicyAsync(CreatePolicyDto policy);

    Task<Policy?> UpdatePolicyAsync(int id, UpdatePolicyDto policy);

    Task<Policy?> UpdatePolicyStatusAsync(int id, UpdatePolicyStatusDto statusUpdate);

    Task<Policy?> DeletePolicyAsync(int id);

}