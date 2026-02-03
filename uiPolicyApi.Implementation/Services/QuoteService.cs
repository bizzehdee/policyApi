using uiPolicyApi.Data.Repositories;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Quote;
using uiPolicyApi.SDK.Services;

namespace uiPolicyApi.Implementation.Services;

public class QuoteService : IQuoteService
{
    public QuoteService(IQuoteRepository quoteRepository)
    {
        
    }
    
    public Task<ResultModel<QuoteModel>> CreateQuoteAsync(QuoteRequestModel quoteRequest)
    {
        throw new NotImplementedException();
    }

    public Task<ResultModel<QuoteModel>> GetQuoteDetailsAsync(uint quoteId)
    {
        throw new NotImplementedException();
    }
}