namespace uiPolicyApi.SDK.Models.Policy;

public class PolicyHolderModel
{
    public uint Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
}