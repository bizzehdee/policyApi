using MediatR;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.SDK.Queries;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.Implementation.Handlers;

public class GetQuoteDetailsQueryHandler : IRequestHandler<GetQuoteDetailsQuery, ResultModel<QuoteModel>>
{
    private readonly IQuoteRepository _quoteRepository;

    public GetQuoteDetailsQueryHandler(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }

    public async Task<ResultModel<QuoteModel>> Handle(GetQuoteDetailsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual quote retrieval logic
        throw new NotImplementedException("GetQuoteDetailsQuery handler not yet fully implemented");
    }
}

