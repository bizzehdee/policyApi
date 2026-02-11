using MediatR;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.SDK.Queries;

public class GetQuoteDetailsQuery : IRequest<ResultModel<QuoteModel>>
{
    public uint QuoteId { get; set; }
    
    public GetQuoteDetailsQuery(uint quoteId)
    {
        QuoteId = quoteId;
    }
}

