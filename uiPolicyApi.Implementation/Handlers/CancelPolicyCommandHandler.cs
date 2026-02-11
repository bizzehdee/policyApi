using MediatR;
using uiPolicyApi.Data.Entities;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.SDK.Commands;
using uiPolicyApi.SDK.Enums;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.Implementation.Handlers;

public class CancelPolicyCommandHandler : IRequestHandler<CancelPolicyCommand, ResultModel<CancelPolicyResultModel>>
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CancelPolicyCommandHandler(IPolicyRepository policyRepository, IPaymentRepository paymentRepository)
    {
        _policyRepository = policyRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<ResultModel<CancelPolicyResultModel>> Handle(CancelPolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _policyRepository.GetPolicyAsync(request.PolicyId);
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
        
        var cancelResult = await _policyRepository.CancelPolicyAsync(request.PolicyId);
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
        var refundType = PaymentType.None;
        
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
}

