using Microsoft.AspNetCore.Mvc;
using TradeSimulation.API.Services;

namespace TradeSimulation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly DhanProxyService _dhanService;

    public StocksController(DhanProxyService dhanService)
    {
        _dhanService = dhanService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStocks(
        [FromQuery] string sort = "Mcap",
        [FromQuery] string order = "desc")
    {
        var stocks = await _dhanService.GetNifty50StocksAsync(sort, order);
        return Ok(stocks);
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> GetStock(string symbol)
    {
        var stock = await _dhanService.GetStockBySymbolAsync(symbol);
        if (stock == null) return NotFound(new { message = $"Stock '{symbol}' not found" });
        return Ok(stock);
    }
}
