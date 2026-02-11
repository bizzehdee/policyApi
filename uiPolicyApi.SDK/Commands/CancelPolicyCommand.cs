using MediatR;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.SDK.Commands;

public class CancelPolicyCommand : IRequest<ResultModel<CancelPolicyResultModel>>
{
    public uint PolicyId { get; set; }
    
    public CancelPolicyCommand(uint policyId)
    {
        PolicyId = policyId;
    }
}

