using uiPolicyApi.SDK.Models.Payments;

namespace uiPolicyApi.SDK.Models.Policy;

public class PolicyModel
{
    
    public uint Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Amount { get; set; }
    public bool AutoRenew { get; set; }

    public PolicyPropertyModel PolicyProperty { get; set; } = new PolicyPropertyModel();
    public ICollection<PolicyHolderModel> PolicyHolders { get; set; } = new List<PolicyHolderModel>();
    public ICollection<PaymentModel> Payments { get; set; } = new List<PaymentModel>();
    public ICollection<RefundModel> Refunds { get; set; } = new List<RefundModel>();
}