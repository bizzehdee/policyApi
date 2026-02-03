using uiPolicyApi.SDK.Enums;

namespace uiPolicyApi.Data.Entities;

public class PaymentEntity
{
    public uint Id { get; set; }
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public uint PolicyId { get; set; }
}