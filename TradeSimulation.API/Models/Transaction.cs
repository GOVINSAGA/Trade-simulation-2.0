using System.Text.Json.Serialization;

namespace TradeSimulation.API.Models;

public enum TransactionType
{
    Deposit,
    Withdrawal,
    StockBuy,
    StockSell
}

public class Transaction
{
    public int Id { get; set; }
    [JsonIgnore] public string UserId { get; set; } = string.Empty;
    [JsonIgnore] public AppUser User { get; set; } = null!;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
