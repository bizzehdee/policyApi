using uiPolicyApi.Data.Entities;

namespace uiPolicyApi.Data.Repositories;

public class PaymentRepository : IPaymentRepository
{
    public Task<PaymentEntity?> AddPaymentToPolicyAsync(uint policyId, PaymentEntity payment)
    {
        throw new NotImplementedException();
    }

    public Task<RefundEntity?> AddRefundToPolicyAsync(uint policyId, RefundEntity refundEntity)
    {
        throw new NotImplementedException();
    }
}

