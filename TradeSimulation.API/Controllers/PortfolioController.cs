using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeSimulation.API.Services;

namespace TradeSimulation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly PortfolioService _portfolioService;

    public PortfolioController(PortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetPortfolio()
    {
        var holdings = await _portfolioService.GetPortfolioAsync(UserId);
        return Ok(holdings);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _portfolioService.GetPortfolioSummaryAsync(UserId);
        return Ok(summary);
    }
}
