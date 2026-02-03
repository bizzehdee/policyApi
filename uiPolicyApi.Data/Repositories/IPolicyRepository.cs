using uiPolicyApi.Data.Entities;

namespace uiPolicyApi.Data.Repositories;

public interface IPolicyRepository
{
    Task<IEnumerable<PolicyEntity>> GetAllPoliciesAsync();
    
    Task<PolicyEntity?> GetPolicyAsync(uint policyId);
    Task<PolicyEntity?> AddPolicyAsync(PolicyEntity policy);
    Task<PolicyEntity?> UpdatePolicyAsync(PolicyEntity policy);
    Task<bool> DeletePolicyAsync(uint policyId);
    
    Task<PolicyEntity?> AddPropertyToPolicyAsync(uint policyId, PolicyPropertyEntity property);
    Task<PolicyEntity?> AddHolderToPolicyAsync(uint policyId, PolicyHolderEntity holder);
    Task<bool> CancelPolicyAsync(uint policyId);
}