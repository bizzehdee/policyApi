using MediatR;
using uiPolicyApi.Data.Repositories;
using uiPolicyApi.SDK.Commands;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.Implementation.Handlers;

public class CreateQuoteCommandHandler : IRequestHandler<CreateQuoteCommand, ResultModel<QuoteModel>>
{
    private readonly IQuoteRepository _quoteRepository;

    public CreateQuoteCommandHandler(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }

    public async Task<ResultModel<QuoteModel>> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual quote creation logic
        throw new NotImplementedException("CreateQuoteCommand handler not yet fully implemented");
    }
}

