using uiPolicyApi.Data.Entities;
using uiPolicyApi.SDK.Models.Payments;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.Implementation.Helpers;

public static class MapperHelpers
{
    public static PolicyModel MapPolicyEntityToModel(PolicyEntity policy)
    {
        return new PolicyModel
        {
            Id = policy.Id,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate,
            Amount = policy.Amount,
            AutoRenew = policy.AutoRenew,
            PolicyProperty = MapPropertyEntityToModel(policy.PolicyProperty),
            PolicyHolders = policy.PolicyHolders.Select(MapPolicyHolderEntityToModel).ToList(),
            Payments = policy.Payments.Select(MapPaymentEntityToModel).ToList(),
            Refunds = policy.Refunds.Select(MapRefundEntityToModel).ToList()
        };
    }
    
    public static PolicyEntity MapPolicyModelToEntity(PolicyModel policy)
    {
        return new PolicyEntity
        {
            Id = policy.Id,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate,
            Amount = policy.Amount,
            AutoRenew = policy.AutoRenew,
            PolicyProperty = MapPropertyModelToEntity(policy.PolicyProperty),
            PolicyHolders = policy.PolicyHolders.Select(MapPolicyHolderModelToEntity).ToList(),
            Payments = policy.Payments.Select(MapPaymentModelToEntity).ToList(),
            Refunds = policy.Refunds.Select(MapRefundModelToEntity).ToList()
        };
    }

    public static PolicyHolderModel MapPolicyHolderEntityToModel(PolicyHolderEntity policyHolder)
    {
        return new PolicyHolderModel
        {
            Id = policyHolder.Id,
            FirstName = policyHolder.FirstName,
            LastName = policyHolder.LastName,
            DateOfBirth = policyHolder.DateOfBirth
        };
    }

    public static PolicyHolderEntity MapPolicyHolderModelToEntity(PolicyHolderModel policyHolder)
    {
        return new PolicyHolderEntity
        {
            Id = policyHolder.Id,
            FirstName = policyHolder.FirstName,
            LastName = policyHolder.LastName,
            DateOfBirth = policyHolder.DateOfBirth
        };
    }

    public static PolicyPropertyModel MapPropertyEntityToModel(PolicyPropertyEntity property)
    {
        return new PolicyPropertyModel
        {
            Id = property.Id,
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            AddressLine3 = property.AddressLine3,
            PostCode = property.PostCode
        };
    }

    public static PolicyPropertyEntity MapPropertyModelToEntity(PolicyPropertyModel property)
    {
        return new PolicyPropertyEntity
        {
            Id = property.Id,
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            AddressLine3 = property.AddressLine3,
            PostCode = property.PostCode
        };
    }
    
    public static PaymentModel MapPaymentEntityToModel(PaymentEntity payment)
    {
        return new PaymentModel
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Type = payment.Type,
            CreatedAt = payment.CreatedAt
        };
    }
    
    public static PaymentEntity MapPaymentModelToEntity(PaymentModel payment)
    {
        return new PaymentEntity
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Type = payment.Type,
            CreatedAt = payment.CreatedAt
        };
    }
    
    public static RefundModel MapRefundEntityToModel(RefundEntity refund)
    {
        return new RefundModel
        {
            Id = refund.Id,
            Amount = refund.Amount,
            Type = refund.Type,
            CreatedAt = refund.CreatedAt
        };
    }
    
    public static RefundEntity MapRefundModelToEntity(RefundModel refund)
    {
        return new RefundEntity
        {
            Id = refund.Id,
            Amount = refund.Amount,
            Type = refund.Type,
            CreatedAt = refund.CreatedAt
        };
    }
}