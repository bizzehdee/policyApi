using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.SDK.Services;

public interface IPolicyService
{
    Task<ResultModel<PolicyModel>> GetPolicyDetailsAsync(uint policyId);
    Task<ResultModel<PolicyModel>> CreatePolicyAsync(QuoteModel quote);
    Task<ResultModel<RenewPolicyResultModel>> RenewPolicyAsync(uint policyId);
    Task<ResultModel<CancelPolicyResultModel>> CancelPolicyAsync(uint policyId);
    Task<ResultModel<bool>> IsPolicyAssociatedWithUserAsync(uint policyId, uint userId);
}
