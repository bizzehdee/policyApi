using MediatR;
using uiPolicyApi.Data.Entities;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.Implementation.Helpers;
using uiPolicyApi.SDK.Commands;
using uiPolicyApi.SDK.Enums;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.Implementation.Handlers;

public class RenewPolicyCommandHandler : IRequestHandler<RenewPolicyCommand, ResultModel<RenewPolicyResultModel>>
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IPaymentRepository _paymentRepository;

    public RenewPolicyCommandHandler(IPolicyRepository policyRepository, IPaymentRepository paymentRepository)
    {
        _policyRepository = policyRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<ResultModel<RenewPolicyResultModel>> Handle(RenewPolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _policyRepository.GetPolicyAsync(request.PolicyId);
        if (policy == null)
        {
            return new ResultModel<RenewPolicyResultModel>
            {
                Success = false,
                Message = "Policy not found"
            };
        }

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        // has the policy already expired?
        if (currentDate > policy.EndDate)
        {
            return new ResultModel<RenewPolicyResultModel>
            {
                Success = false,
                Message = "Policy has already expired and cannot be renewed"
            };
        }
        
        // is it within 30 days of expiration?
        if(currentDate < policy.EndDate.AddDays(-30))
        {
            return new ResultModel<RenewPolicyResultModel>
            {
                Success = false,
                Message = "Policy can only be renewed within 30 days of its expiration date"
            };
        }
        
        // copy the existing policy details to create a new policy
        var newPolicy = new PolicyModel
        {
            StartDate = policy.EndDate.AddDays(1),
            EndDate = policy.EndDate.AddYears(1),
            Amount = policy.Amount,
            AutoRenew = policy.AutoRenew,
            PolicyHolders = new List<PolicyHolderModel>(),
            PolicyProperty = new PolicyPropertyModel
            {
                Id = policy.Id,
                AddressLine1 = policy.PolicyProperty.AddressLine1,
                AddressLine2 = policy.PolicyProperty.AddressLine2,
                AddressLine3 = policy.PolicyProperty.AddressLine3,
                PostCode = policy.PolicyProperty.PostCode
            }
        };
        
        // save the new policy
        var policyResult = await CreatePolicyInternalAsync(newPolicy, policy.AutoRenew);
        
        if (!policyResult.Success)
        {
            return new ResultModel<RenewPolicyResultModel>
            {
                Success = false,
                Message = "Failed to create renewed policy: " + policyResult.Message
            };
        }
        
        return new ResultModel<RenewPolicyResultModel>
        {
            Success = true,
            Result = new RenewPolicyResultModel
            {
                PolicyId = policyResult.Result.Id,
                NewStartDate = policyResult.Result.StartDate,
                NewEndDate = policyResult.Result.EndDate,
                NewAmount = policyResult.Result.Amount,
                PaymentRaised = true
            }
        };
    }

    private async Task<ResultModel<PolicyModel>> CreatePolicyInternalAsync(PolicyModel policyModel, bool raisePayment)
    {
        // create record in the database
        var policyWithId = await _policyRepository.AddPolicyAsync(MapperHelpers.MapPolicyModelToEntity(policyModel));

        if (policyWithId == null)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Failed to create policy in database"
            };
        }
        
        // optionally raise payment
        if (raisePayment)
        {
            // create payment record
            await _paymentRepository.AddPaymentToPolicyAsync(policyWithId.Id, new PaymentEntity
            {
                Amount = policyWithId.Amount,
                CreatedAt = DateTime.UtcNow,
                Type = PaymentType.DirectDebit,
            });
        }
        
        return new ResultModel<PolicyModel>
        {
            Success = true, 
            Result = MapperHelpers.MapPolicyEntityToModel(policyWithId)
        };
    }
}

