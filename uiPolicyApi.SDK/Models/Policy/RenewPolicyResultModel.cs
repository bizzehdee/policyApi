namespace uiPolicyApi.SDK.Models.Policy;

public class RenewPolicyResultModel
{
    public uint PolicyId { get; set; }
    public DateOnly NewStartDate { get; set; }
    public DateOnly NewEndDate { get; set; }
    public decimal NewAmount { get; set; }
    public bool PaymentRaised { get; set; }
}