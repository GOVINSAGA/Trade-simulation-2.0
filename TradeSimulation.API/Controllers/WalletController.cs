using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeSimulation.API.Models;
using TradeSimulation.API.Services;

namespace TradeSimulation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly WalletService _walletService;

    public WalletController(WalletService walletService)
    {
        _walletService = walletService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        var wallet = await _walletService.GetWalletAsync(UserId);
        return Ok(wallet);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        try
        {
            var wallet = await _walletService.DepositAsync(UserId, request.Amount);
            return Ok(wallet);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        try
        {
            var wallet = await _walletService.WithdrawAsync(UserId, request.Amount);
            return Ok(wallet);
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
}
