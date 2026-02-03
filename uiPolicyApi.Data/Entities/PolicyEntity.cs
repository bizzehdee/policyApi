using System.Runtime.CompilerServices;

namespace uiPolicyApi.Data.Entities;

public class PolicyEntity
{
    public uint Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Amount { get; set; }
    public bool AutoRenew { get; set; }

    public virtual PolicyPropertyEntity PolicyProperty { get; set; } = new PolicyPropertyEntity();
    public virtual ICollection<PolicyHolderEntity> PolicyHolders { get; set; } = new List<PolicyHolderEntity>();
    public virtual ICollection<PaymentEntity> Payments { get; set; } = new List<PaymentEntity>();
    public virtual ICollection<RefundEntity> Refunds { get; set; } = new List<RefundEntity>();
}