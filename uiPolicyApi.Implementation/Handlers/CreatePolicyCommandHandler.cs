using MediatR;
using uiPolicyApi.Data.Entities;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.Implementation.Helpers;
using uiPolicyApi.SDK.Commands;
using uiPolicyApi.SDK.Enums;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.Implementation.Handlers;

public class CreatePolicyCommandHandler : IRequestHandler<CreatePolicyCommand, ResultModel<PolicyModel>>
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CreatePolicyCommandHandler(IPolicyRepository policyRepository, IPaymentRepository paymentRepository)
    {
        _policyRepository = policyRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<ResultModel<PolicyModel>> Handle(CreatePolicyCommand request, CancellationToken cancellationToken)
    {
        var quote = request.Quote;
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
        var createPolicyResult = await CreatePolicyInternalAsync(new PolicyModel
        {
            StartDate = quote.StartDate,
            EndDate = quote.EndDate,
            Amount = quote.Amount,
            AutoRenew = true
        }, true);

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

