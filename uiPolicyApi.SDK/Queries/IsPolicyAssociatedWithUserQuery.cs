using MediatR;
using uiPolicyApi.SDK.Models;

namespace uiPolicyApi.SDK.Queries;

public class IsPolicyAssociatedWithUserQuery : IRequest<ResultModel<bool>>
{
    public uint PolicyId { get; set; }
    public uint UserId { get; set; }
    
    public IsPolicyAssociatedWithUserQuery(uint policyId, uint userId)
    {
        PolicyId = policyId;
        UserId = userId;
    }
}

