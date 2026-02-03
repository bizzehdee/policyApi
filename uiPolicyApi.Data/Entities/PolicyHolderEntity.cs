namespace uiPolicyApi.Data.Entities;

public class PolicyHolderEntity
{
    public uint Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    
    public uint PolicyId { get; set; }
}