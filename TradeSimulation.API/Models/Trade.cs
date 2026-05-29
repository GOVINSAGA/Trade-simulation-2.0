using System.Text.Json.Serialization;

namespace TradeSimulation.API.Models;

public enum TradeType
{
    Buy,
    Sell
}

public class Trade
{
    public int Id { get; set; }
    [JsonIgnore] public string UserId { get; set; } = string.Empty;
    [JsonIgnore] public AppUser User { get; set; } = null!;
    public string Symbol { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PricePerShare { get; set; }
    public decimal TotalAmount { get; set; }
    public TradeType Type { get; set; }
    public string Sector { get; set; } = string.Empty;
    public DateTime TradedAt { get; set; } = DateTime.UtcNow;
}
