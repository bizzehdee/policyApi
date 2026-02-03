using Microsoft.AspNetCore.Mvc;
using uiPolicyApi.SDK.Services;

namespace uiPolicyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PolicyController : ControllerBase
{
    private readonly IPolicyService _policyService;
    private readonly IQuoteService _quoteService;

    public PolicyController(IPolicyService policyService, IQuoteService quoteService)
    {
        _policyService = policyService;
        _quoteService = quoteService;
    }
    
    [HttpGet("{policyId}")]
    public async Task<IActionResult> GetPolicyDetails(uint policyId)
    {
        // validate the user has access to the policy
        // if not, return Unauthorized();
        var accessResult = await _policyService.IsPolicyAssociatedWithUserAsync(policyId, /* get user id from context */ 0);
        if (!accessResult.Success || !accessResult.Result)
        {
            return Unauthorized();
        }
        
        // otherwise, get the policy details
        var policyResult = await _policyService.GetPolicyDetailsAsync(policyId);
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
        var quoteResult = await _quoteService.GetQuoteDetailsAsync(quoteId);
        if (!quoteResult.Success)
        {
            return BadRequest(quoteResult.Message);
        }

        // create the policy
        var policyResult = await _policyService.CreatePolicyAsync(quoteResult.Result);
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
        var accessResult = await _policyService.IsPolicyAssociatedWithUserAsync(policyId, /* get user id from context */ 0);
        if (!accessResult.Success || !accessResult.Result)
        {
            return Unauthorized();
        }
        
        // otherwise, attempt to renew the policy
        var renewalResult = await _policyService.RenewPolicyAsync(policyId);
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
        var accessResult = await _policyService.IsPolicyAssociatedWithUserAsync(policyId, /* get user id from context */ 0);
        if (!accessResult.Success || !accessResult.Result)
        {
            return Unauthorized();
        }
        
        // otherwise, attempt to cancel the policy
        var renewalResult = await _policyService.CancelPolicyAsync(policyId);
        if (renewalResult.Success)
        {
            return Ok(renewalResult.Result);
        }
        
        return BadRequest(renewalResult.Message);
    }
}