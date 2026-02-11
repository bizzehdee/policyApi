using MediatR;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Policy;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.SDK.Commands;

public class CreatePolicyCommand : IRequest<ResultModel<PolicyModel>>
{
    public QuoteModel Quote { get; set; }
    
    public CreatePolicyCommand(QuoteModel quote)
    {
        Quote = quote;
    }
}

