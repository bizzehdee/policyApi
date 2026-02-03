using uiPolicyApi.SDK.Enums;

namespace uiPolicyApi.Data.Entities;

public class RefundEntity
{
    public uint Id { get; set; }
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public uint PolicyId { get; set; }
    public string Reason { get; set; }
}
