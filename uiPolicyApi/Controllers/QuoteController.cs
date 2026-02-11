using MediatR;
using Microsoft.AspNetCore.Mvc;
using uiPolicyApi.SDK.Commands;
using uiPolicyApi.SDK.Models.Quote;

namespace uiPolicyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class QuoteController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuoteController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateQuote(QuoteRequestModel quoteRequest)
    {
        var quoteResult = await _mediator.Send(new CreateQuoteCommand(quoteRequest));
        if (quoteResult.Success)
        {
            return Ok(quoteResult.Result);
        }
        
        return BadRequest(quoteResult.Message);
    }
}