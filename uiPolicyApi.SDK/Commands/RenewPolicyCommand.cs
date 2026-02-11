using MediatR;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.SDK.Commands;

public class RenewPolicyCommand : IRequest<ResultModel<RenewPolicyResultModel>>
{
    public uint PolicyId { get; set; }
    
    public RenewPolicyCommand(uint policyId)
    {
        PolicyId = policyId;
    }
}

