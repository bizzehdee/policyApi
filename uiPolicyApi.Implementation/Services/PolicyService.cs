using uiPolicyApi.Data.Entities;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.Implementation.Helpers;
using uiPolicyApi.SDK.Enums;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Payments;
using uiPolicyApi.SDK.Models.Policy;
using uiPolicyApi.SDK.Models.Quote;
using uiPolicyApi.SDK.Services;

namespace uiPolicyApi.Implementation.Services;

public class PolicyService : IPolicyService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IPaymentRepository _paymentRepository;

    public PolicyService(IPolicyRepository policyRepository, IPaymentRepository paymentRepository)
    {
        _policyRepository = policyRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<ResultModel<PolicyModel>> GetPolicyDetailsAsync(uint policyId)
    {
        var policy = await _policyRepository.GetPolicyAsync(policyId);
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

    public async Task<ResultModel<PolicyModel>> CreatePolicyAsync(QuoteModel quote)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        
        //start date is in the past
        if (quote.StartDate < now)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Policy start date cannot be in the past"
            };
        }
        
        //start date is more than 60 days in the future
        if(quote.StartDate > now.AddDays(60))
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Policy start date cannot be more than 60 days in the future"
            };
        }
        
        // end date must me 1 year on from the start date
        if (quote.StartDate.AddYears(1).AddDays(-1) != quote.EndDate)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Policy start date cannot be more than 60 days in the future"
            };
        }
        
        // must have at least 1 policy holder
        if (quote.PolicyHolders.Count == 0)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Policy must have at least one policy holder"
            };
        }
        
        // must have no more than 3 policy holders
        if (quote.PolicyHolders.Count > 3)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Policy cannot have more than three policy holders"
            };
        }
        
        // all policy holders must be at least 16 years old on the start date
        foreach (var holder in quote.PolicyHolders)
        {
            var age = quote.StartDate.Year - holder.DateOfBirth.Year;
            if (holder.DateOfBirth > quote.StartDate.AddYears(-age)) age--;
            if (age < 16)
            {
                return new ResultModel<PolicyModel>
                {
                    Success = false,
                    Message = "All policy holders must be at least 16 years old on the policy start date"
                };
            }
        }
        
        //create policy, which will generate the new ID in the DB
        var createPolicyResult = await CreatePolicyInternalAsync((new PolicyModel
        {
            StartDate = quote.StartDate,
            EndDate = quote.EndDate,
            Amount = quote.Amount,
            AutoRenew = true
        }), true);

        // did we create the policy?
        if (!createPolicyResult.Success)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Failed to create policy"
            };
        }

        var policyWithId = createPolicyResult.Result;

        // add the property to the policy
        await _policyRepository.AddPropertyToPolicyAsync(policyWithId.Id, MapperHelpers.MapPropertyModelToEntity(new PolicyPropertyModel
        {
            AddressLine1 = quote.PolicyProperty.AddressLine1,
            AddressLine2 = quote.PolicyProperty.AddressLine2,
            AddressLine3 = quote.PolicyProperty.AddressLine3,
            PostCode = quote.PolicyProperty.PostCode
        }));
        
        // add each policy holder
        foreach (var holder in quote.PolicyHolders)
        {
            await _policyRepository.AddHolderToPolicyAsync(policyWithId.Id, MapperHelpers.MapPolicyHolderModelToEntity(new PolicyHolderModel
            {
                FirstName = holder.FirstName,
                LastName = holder.LastName,
                DateOfBirth = holder.DateOfBirth
            }));
        }
        
        // refetch the policy to get all the details
        var fetchedPolicyWithId = await _policyRepository.GetPolicyAsync(policyWithId.Id);
        if (fetchedPolicyWithId == null)
        {
            return new ResultModel<PolicyModel>
            {
                Success = false,
                Message = "Failed to retrieve created policy"
            };
        }
        
        // map to model and return
        var policyModel = MapperHelpers.MapPolicyEntityToModel(fetchedPolicyWithId);
        return new ResultModel<PolicyModel>
        {
            Success = true,
            Result = policyModel
        };
    }

    public async Task<ResultModel<RenewPolicyResultModel>> RenewPolicyAsync(uint policyId)
    {
        var policy = await _policyRepository.GetPolicyAsync(policyId);
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

    public async Task<ResultModel<CancelPolicyResultModel>> CancelPolicyAsync(uint policyId)
    {
        var policy = await _policyRepository.GetPolicyAsync(policyId);
        if (policy == null)
        {
            return new ResultModel<CancelPolicyResultModel>
            {
                Success = false,
                Message = "Policy not found"
            };
        }

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        if(currentDate >= policy.EndDate)
        {
            return new ResultModel<CancelPolicyResultModel>
            {
                Success = false,
                Message = "Policy has already expired and cannot be cancelled"
            };
        }
        
        var cancelResult = await _policyRepository.CancelPolicyAsync(policyId);
        if (!cancelResult)
        {
            return new ResultModel<CancelPolicyResultModel>
            {
                Success = false,
                Message = "Failed to cancel policy"
            };
        }
        
        var refundPercentage = 1.0m;

        if (currentDate >= policy.StartDate.AddDays(14))
        {
            // after the cooling off period, pro-rata refund
            var daysRemaining = (policy.EndDate.ToDateTime(TimeOnly.MinValue) - currentDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
            var totalDays = (policy.EndDate.ToDateTime(TimeOnly.MinValue) - policy.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
            refundPercentage = Math.Round((decimal)daysRemaining / totalDays, 2);
        }
        
        // create refund record
        var totalPaymentAmount = 0m;
        var refundType = SDK.Enums.PaymentType.None;
        
        foreach (var payment in policy.Payments)
        {
            totalPaymentAmount += payment.Amount;
            if (refundType == PaymentType.None)
            {
                refundType = payment.Type;
            }
        }
        
        var refundAmount = Math.Round(totalPaymentAmount * refundPercentage, 2);
        
        // trigger something like SQS or service bus here in a real system
        // which would be picked up by a payment service to process the refund
        // and call the payment gateway API
        
        await _paymentRepository.AddRefundToPolicyAsync(policy.Id, new RefundEntity
        {
            Amount = refundAmount,
            CreatedAt = DateTime.UtcNow,
            Type = refundType,
            Reason = "Policy cancellation"
        });
        
        return new ResultModel<CancelPolicyResultModel>
        {
            Success = true,
            Message = "Policy Cancelled and Refund has been raised"
        };
    }

    public async Task<ResultModel<bool>> IsPolicyAssociatedWithUserAsync(uint policyId, uint userId)
    {
        return new ResultModel<bool>
        {
            Success = true,
            Result = true
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
                Type = SDK.Enums.PaymentType.DirectDebit,
            });
        }
        
        return new ResultModel<PolicyModel>
        {
            Success = true, 
            Result = MapperHelpers.MapPolicyEntityToModel(policyWithId)
        };
    }
}