using MediatR;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.SDK.Commands;

public class CreateQuoteCommand : IRequest<ResultModel<QuoteModel>>
{
    public QuoteRequestModel QuoteRequest { get; set; }
    
    public CreateQuoteCommand(QuoteRequestModel quoteRequest)
    {
        QuoteRequest = quoteRequest;
    }
}

