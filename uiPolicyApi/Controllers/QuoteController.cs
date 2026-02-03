using Microsoft.AspNetCore.Mvc;
using uiPolicyApi.SDK.Models;
using uiPolicyApi.SDK.Models.Quote;
using uiPolicyApi.SDK.Services;

namespace uiPolicyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class QuoteController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuoteController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateQuote(QuoteRequestModel quoteRequest)
    {
        var quoteResult = await _quoteService.CreateQuoteAsync(quoteRequest);
        if (quoteResult.Success)
        {
            return Ok(quoteResult.Result);
        }
        
        return BadRequest(quoteResult.Message);
    }
}