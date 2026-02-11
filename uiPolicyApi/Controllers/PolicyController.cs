using MediatR;
using Microsoft.AspNetCore.Mvc;
using uiPolicyApi.SDK.Commands;
using uiPolicyApi.SDK.Queries;

namespace uiPolicyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PolicyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PolicyController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{policyId}")]
    public async Task<IActionResult> GetPolicyDetails(uint policyId)
    {
        // validate the user has access to the policy
        // if not, return Unauthorized();
        var accessResult = await _mediator.Send(new IsPolicyAssociatedWithUserQuery(policyId, /* get user id from context */ 0));
        if (!accessResult.Success || !accessResult.Result)
        {
            return Unauthorized();
        }
        
        // otherwise, get the policy details
        var policyResult = await _mediator.Send(new GetPolicyDetailsQuery(policyId));
        if (policyResult.Success)
        {
            return Ok(policyResult.Result);
        }
    
        return BadRequest(policyResult.Message);
    }
    
    [HttpPost("confirm/{quoteId}")]
    public async Task<IActionResult> ConfirmPolicy(uint quoteId)
    {
        // validate the user has access to the quote
        // if not, return Unauthorized();
        
        // otherwise, get the quote details and create the policy
        var quoteResult = await _mediator.Send(new GetQuoteDetailsQuery(quoteId));
        if (!quoteResult.Success)
        {
            return BadRequest(quoteResult.Message);
        }

        // create the policy
        var policyResult = await _mediator.Send(new CreatePolicyCommand(quoteResult.Result));
        if (policyResult.Success)
        {
            return Ok(policyResult.Result);
        }
    
        return BadRequest(policyResult.Message);
    }
    
    [HttpPost("renew/{policyId}")]
    public async Task<IActionResult> RenewPolicy(uint policyId)
    {
        // validate the user has access to the quote
        // if not, return Unauthorized();
        var accessResult = await _mediator.Send(new IsPolicyAssociatedWithUserQuery(policyId, /* get user id from context */ 0));
        if (!accessResult.Success || !accessResult.Result)
        {
            return Unauthorized();
        }
        
        // otherwise, attempt to renew the policy
        var renewalResult = await _mediator.Send(new RenewPolicyCommand(policyId));
        if (renewalResult.Success)
        {
            return Ok(renewalResult.Result);
        }
        
        return BadRequest(renewalResult.Message);
    }
    
    [HttpPost("cancel/{policyId}")]
    public async Task<IActionResult> CancelPolicy(uint policyId)
    {
        // validate the user has access to the quote
        // if not, return Unauthorized();
        var accessResult = await _mediator.Send(new IsPolicyAssociatedWithUserQuery(policyId, /* get user id from context */ 0));
        if (!accessResult.Success || !accessResult.Result)
        {
            return Unauthorized();
        }
        
        // otherwise, attempt to cancel the policy
        var cancelResult = await _mediator.Send(new CancelPolicyCommand(policyId));
        if (cancelResult.Success)
        {
            return Ok(cancelResult.Result);
        }
        
        return BadRequest(cancelResult.Message);
    }
}