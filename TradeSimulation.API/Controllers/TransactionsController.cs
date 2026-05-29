using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSimulation.API.Data;

namespace TradeSimulation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext db)
    {
        _db = db;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions = await _db.Transactions
            .Where(t => t.UserId == UserId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        return Ok(transactions);
    }
}
