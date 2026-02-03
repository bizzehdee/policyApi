using uiPolicyApi.SDK.Models.Payments;
using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.SDK.Models.Quote;

public class QuoteModel
{
    public uint Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Amount { get; set; }

    public PolicyPropertyModel PolicyProperty { get; set; } = new PolicyPropertyModel();
    public ICollection<PolicyHolderModel> PolicyHolders { get; set; } = new List<PolicyHolderModel>();
}