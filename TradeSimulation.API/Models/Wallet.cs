using System.Text.Json.Serialization;

namespace TradeSimulation.API.Models;

public class Wallet
{
    public int Id { get; set; }
    [JsonIgnore] public string UserId { get; set; } = string.Empty;
    [JsonIgnore] public AppUser User { get; set; } = null!;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
