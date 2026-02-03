using uiPolicyApi.SDK.Models.Policy;

namespace uiPolicyApi.SDK.Models.Quote;

public class QuoteRequestModel
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Amount { get; set; }
    
    public PolicyPropertyModel PolicyProperty { get; set; }
    public ICollection<PolicyHolderModel> PolicyHolders { get; set; }
}