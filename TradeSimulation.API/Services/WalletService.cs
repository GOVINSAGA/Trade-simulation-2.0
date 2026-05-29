using Microsoft.EntityFrameworkCore;
using TradeSimulation.API.Data;
using TradeSimulation.API.Models;

namespace TradeSimulation.API.Services;

public class WalletService
{
    private readonly AppDbContext _db;

    public WalletService(AppDbContext db)
    {
        _db = db;
    }

    public async Task InitializeWalletForUserAsync(string userId)
    {
        var wallet = new Wallet { UserId = userId, Balance = 1000000m };
        _db.Wallets.Add(wallet);

        _db.Transactions.Add(new Transaction
        {
            UserId = userId,
            Type = TransactionType.Deposit,
            Amount = 1000000m,
            Description = "Initial wallet balance (₹10,00,000)"
        });

        await _db.SaveChangesAsync();
    }

    public async Task<Wallet> GetWalletAsync(string userId)
    {
        var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        return wallet!;
    }

    public async Task<Wallet> DepositAsync(string userId, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Deposit amount must be positive");

        var wallet = await GetWalletAsync(userId);
        wallet.Balance += amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _db.Transactions.Add(new Transaction
        {
            UserId = userId,
            Type = TransactionType.Deposit,
            Amount = amount,
            Description = $"Deposited ₹{amount:N2}"
        });

        await _db.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet> WithdrawAsync(string userId, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Withdrawal amount must be positive");

        var wallet = await GetWalletAsync(userId);
        if (wallet.Balance < amount) throw new InvalidOperationException("Insufficient balance");

        wallet.Balance -= amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _db.Transactions.Add(new Transaction
        {
            UserId = userId,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            Description = $"Withdrew ₹{amount:N2}"
        });

        await _db.SaveChangesAsync();
        return wallet;
    }

    public async Task DebitAsync(string userId, decimal amount, string description)
    {
        var wallet = await GetWalletAsync(userId);
        if (wallet.Balance < amount) throw new InvalidOperationException("Insufficient balance");

        wallet.Balance -= amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _db.Transactions.Add(new Transaction
        {
            UserId = userId,
            Type = TransactionType.StockBuy,
            Amount = amount,
            Description = description
        });

        await _db.SaveChangesAsync();
    }

    public async Task CreditAsync(string userId, decimal amount, string description)
    {
        var wallet = await GetWalletAsync(userId);
        wallet.Balance += amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _db.Transactions.Add(new Transaction
        {
            UserId = userId,
            Type = TransactionType.StockSell,
            Amount = amount,
            Description = description
        });

        await _db.SaveChangesAsync();
    }
}
