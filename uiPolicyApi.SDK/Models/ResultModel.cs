namespace uiPolicyApi.SDK.Models;

public class ResultModel<T>
{
    public T Result { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}