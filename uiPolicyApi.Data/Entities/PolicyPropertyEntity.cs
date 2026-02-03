namespace uiPolicyApi.Data.Entities;

public class PolicyPropertyEntity
{
    public uint Id { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string PostCode { get; set; }
    public uint PolicyId { get; set; }
}