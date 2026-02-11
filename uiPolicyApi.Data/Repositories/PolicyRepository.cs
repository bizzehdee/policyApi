using uiPolicyApi.Data.Entities;

namespace uiPolicyApi.Data.Repositories;

public class PolicyRepository : IPolicyRepository
{
    public Task<IEnumerable<PolicyEntity>> GetAllPoliciesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PolicyEntity?> GetPolicyAsync(uint policyId)
    {
        throw new NotImplementedException();
    }

    public Task<PolicyEntity?> AddPolicyAsync(PolicyEntity policy)
    {
        throw new NotImplementedException();
    }

    public Task<PolicyEntity?> UpdatePolicyAsync(PolicyEntity policy)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePolicyAsync(uint policyId)
    {
        throw new NotImplementedException();
    }

    public Task<PolicyEntity?> AddPropertyToPolicyAsync(uint policyId, PolicyPropertyEntity property)
    {
        throw new NotImplementedException();
    }

    public Task<PolicyEntity?> AddHolderToPolicyAsync(uint policyId, PolicyHolderEntity holder)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CancelPolicyAsync(uint policyId)
    {
        throw new NotImplementedException();
    }
}