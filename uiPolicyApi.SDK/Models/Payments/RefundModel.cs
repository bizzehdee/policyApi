using uiPolicyApi.SDK.Enums;

namespace uiPolicyApi.SDK.Models.Payments;

public class RefundModel
{
    public uint Id { get; set; }
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}