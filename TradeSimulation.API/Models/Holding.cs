using System.Text.Json.Serialization;

namespace TradeSimulation.API.Models;

public class Holding
{
    public int Id { get; set; }
    [JsonIgnore] public string UserId { get; set; } = string.Empty;
    [JsonIgnore] public AppUser User { get; set; } = null!;
    public string Symbol { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal AvgBuyPrice { get; set; }
    public string Sector { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
