using Microsoft.EntityFrameworkCore;
using TradeSimulation.API.Data;
using TradeSimulation.API.Models;

namespace TradeSimulation.API.Services;

public class PortfolioService
{
    private readonly AppDbContext _db;
    private readonly DhanProxyService _dhanService;
    private readonly WalletService _walletService;

    public PortfolioService(AppDbContext db, DhanProxyService dhanService, WalletService walletService)
    {
        _db = db;
        _dhanService = dhanService;
        _walletService = walletService;
    }

    public async Task<List<PortfolioHolding>> GetPortfolioAsync(string userId)
    {
        var holdings = await _db.Holdings.Where(h => h.UserId == userId).ToListAsync();
        if (!holdings.Any()) return new List<PortfolioHolding>();

        var stocks = await _dhanService.GetNifty50StocksAsync();
        var stockMap = stocks.ToDictionary(s => s.Sym, s => s.Ltp);

        return holdings.Select(h =>
        {
            var currentPrice = stockMap.GetValueOrDefault(h.Symbol, h.AvgBuyPrice);
            var invested = h.Quantity * h.AvgBuyPrice;
            var current = h.Quantity * currentPrice;
            var pnl = current - invested;
            var pnlPercent = invested > 0 ? (pnl / invested) * 100 : 0;

            return new PortfolioHolding(
                h.Symbol,
                h.DisplayName,
                h.Quantity,
                h.AvgBuyPrice,
                currentPrice,
                invested,
                current,
                pnl,
                pnlPercent,
                h.Sector
            );
        }).ToList();
    }

    public async Task<PortfolioSummary> GetPortfolioSummaryAsync(string userId)
    {
        var portfolio = await GetPortfolioAsync(userId);
        var wallet = await _walletService.GetWalletAsync(userId);

        var totalInvested = portfolio.Sum(p => p.InvestedValue);
        var currentValue = portfolio.Sum(p => p.CurrentValue);
        var totalPnL = currentValue - totalInvested;
        var totalPnLPercent = totalInvested > 0 ? (totalPnL / totalInvested) * 100 : 0;

        return new PortfolioSummary(
            totalInvested,
            currentValue,
            totalPnL,
            totalPnLPercent,
            portfolio.Count,
            wallet.Balance
        );
    }
}
