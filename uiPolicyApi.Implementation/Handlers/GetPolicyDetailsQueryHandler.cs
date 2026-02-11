using MediatR;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.Implementation.Helpers;
using uiPolicyApi.SDK.Queries;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.Implementation.Handlers;

public class GetPolicyDetailsQueryHandler : IRequestHandler<GetPolicyDetailsQuery, ResultModel<PolicyModel>>
{
    private readonly IPolicyRepository _policyRepository;

    public GetPolicyDetailsQueryHandler(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public async Task<ResultModel<PolicyModel>> Handle(GetPolicyDetailsQuery request, CancellationToken cancellationToken)
    {
        var policy = await _policyRepository.GetPolicyAsync(request.PolicyId);
        if (policy == null)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Policy not found"
            };
        }
        
        return new ResultModel<PolicyModel>
        {
            Success = true,
            Result = MapperHelpers.MapPolicyEntityToModel(policy)
        };
    }
}

