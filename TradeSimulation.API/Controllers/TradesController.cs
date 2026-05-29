using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeSimulation.API.Models;
using TradeSimulation.API.Services;

namespace TradeSimulation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TradesController : ControllerBase
{
    private readonly TradeService _tradeService;

    public TradesController(TradeService tradeService)
    {
        _tradeService = tradeService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost("buy")]
    public async Task<IActionResult> Buy([FromBody] BuyRequest request)
    {
        try
        {
            var trade = await _tradeService.BuyStockAsync(UserId, request);
            return Ok(trade);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("sell")]
    public async Task<IActionResult> Sell([FromBody] SellRequest request)
    {
        try
        {
            var trade = await _tradeService.SellStockAsync(UserId, request);
            return Ok(trade);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var trades = await _tradeService.GetTradeHistoryAsync(UserId);
        return Ok(trades);
    }
}
