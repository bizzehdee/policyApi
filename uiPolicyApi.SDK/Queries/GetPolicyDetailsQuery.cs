using MediatR;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.SDK.Queries;

public class GetPolicyDetailsQuery : IRequest<ResultModel<PolicyModel>>
{
    public uint PolicyId { get; set; }
    
    public GetPolicyDetailsQuery(uint policyId)
    {
        PolicyId = policyId;
    }
}

