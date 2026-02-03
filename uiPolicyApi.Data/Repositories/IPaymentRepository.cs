using uiPolicyApi.Data.Entities;

namespace uiPolicyApi.Data.Repositories;

public interface IPaymentRepository
{
    Task<PaymentEntity?> AddPaymentToPolicyAsync(uint policyId, PaymentEntity payment);
    Task<RefundEntity?> AddRefundToPolicyAsync(uint policyId, RefundEntity refundEntity);
}