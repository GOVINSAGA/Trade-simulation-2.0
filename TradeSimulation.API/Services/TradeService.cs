using Microsoft.EntityFrameworkCore;
using TradeSimulation.API.Data;
using TradeSimulation.API.Models;

namespace TradeSimulation.API.Services;

public class TradeService
{
    private readonly AppDbContext _db;
    private readonly WalletService _walletService;

    public TradeService(AppDbContext db, WalletService walletService)
    {
        _db = db;
        _walletService = walletService;
    }

    public async Task<Trade> BuyStockAsync(string userId, BuyRequest request)
    {
        if (request.Quantity <= 0) throw new ArgumentException("Quantity must be positive");
        if (request.PricePerShare <= 0) throw new ArgumentException("Price must be positive");

        var totalCost = request.Quantity * request.PricePerShare;

        await _walletService.DebitAsync(userId, totalCost,
            $"Bought {request.Quantity} x {request.Symbol} @ ₹{request.PricePerShare:N2}");

        var trade = new Trade
        {
            UserId = userId,
            Symbol = request.Symbol,
            DisplayName = request.DisplayName,
            Quantity = request.Quantity,
            PricePerShare = request.PricePerShare,
            TotalAmount = totalCost,
            Type = TradeType.Buy,
            Sector = request.Sector
        };
        _db.Trades.Add(trade);

        var holding = await _db.Holdings.FirstOrDefaultAsync(h => h.UserId == userId && h.Symbol == request.Symbol);
        if (holding != null)
        {
            var totalShares = holding.Quantity + request.Quantity;
            holding.AvgBuyPrice = ((holding.AvgBuyPrice * holding.Quantity) + totalCost) / totalShares;
            holding.Quantity = totalShares;
            holding.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.Holdings.Add(new Holding
            {
                UserId = userId,
                Symbol = request.Symbol,
                DisplayName = request.DisplayName,
                Quantity = request.Quantity,
                AvgBuyPrice = request.PricePerShare,
                Sector = request.Sector
            });
        }

        await _db.SaveChangesAsync();
        return trade;
    }

    public async Task<Trade> SellStockAsync(string userId, SellRequest request)
    {
        if (request.Quantity <= 0) throw new ArgumentException("Quantity must be positive");
        if (request.PricePerShare <= 0) throw new ArgumentException("Price must be positive");

        var holding = await _db.Holdings.FirstOrDefaultAsync(h => h.UserId == userId && h.Symbol == request.Symbol);
        if (holding == null) throw new InvalidOperationException($"No holding found for {request.Symbol}");
        if (holding.Quantity < request.Quantity)
            throw new InvalidOperationException($"Insufficient shares. You hold {holding.Quantity} of {request.Symbol}");

        var totalValue = request.Quantity * request.PricePerShare;

        await _walletService.CreditAsync(userId, totalValue,
            $"Sold {request.Quantity} x {request.Symbol} @ ₹{request.PricePerShare:N2}");

        var trade = new Trade
        {
            UserId = userId,
            Symbol = request.Symbol,
            DisplayName = holding.DisplayName,
            Quantity = request.Quantity,
            PricePerShare = request.PricePerShare,
            TotalAmount = totalValue,
            Type = TradeType.Sell,
            Sector = holding.Sector
        };
        _db.Trades.Add(trade);

        holding.Quantity -= request.Quantity;
        if (holding.Quantity == 0)
        {
            _db.Holdings.Remove(holding);
        }
        else
        {
            holding.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return trade;
    }

    public async Task<List<Trade>> GetTradeHistoryAsync(string userId)
    {
        return await _db.Trades
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TradedAt)
            .ToListAsync();
    }
}
