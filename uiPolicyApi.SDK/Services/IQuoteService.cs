using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.SDK.Services;

public interface IQuoteService
{
    Task<ResultModel<QuoteModel>> CreateQuoteAsync(QuoteRequestModel quoteRequest);
    Task<ResultModel<QuoteModel>> GetQuoteDetailsAsync(uint quoteId);
}
