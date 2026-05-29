using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TradeSimulation.API.Models;

namespace TradeSimulation.API.Services;

public class DhanProxyService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DhanProxyService> _logger;
    private const string CacheKey = "dhan_stocks";
    private const string ApiUrl = "https://ow-scanx-analytics.dhan.co/customscan/fetchdt";

    public DhanProxyService(HttpClient httpClient, IMemoryCache cache, ILogger<DhanProxyService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<DhanStockData>> GetNifty50StocksAsync(string sort = "Mcap", string order = "desc")
    {
        var cacheKey = $"{CacheKey}_{sort}_{order}";

        if (_cache.TryGetValue(cacheKey, out List<DhanStockData>? cached) && cached != null)
        {
            return cached;
        }

        var payload = new DhanApiRequest(new DhanRequestData(
            sort: sort,
            sorder: order,
            count: 50,
            @params: new List<DhanParam>
            {
                new("idxlist.Indexid", "", "13"),
                new("Exch", "", "NSE"),
                new("OgInst", "", "ES")
            },
            fields: new List<string>
            {
                "Sym", "Mcap", "High1Yr", "Low1Yr", "Pe", "ROCE",
                "PricePerchng1mon", "PricePerchng1year", "Sector",
                "sect_seo", "AnalystRating"
            },
            pgno: 1
        ));

        var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add("origin", "https://dhan.co");
        request.Headers.Add("referer", "https://dhan.co/");
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<DhanApiResponse>(json, options);

            if (result?.data != null)
            {
                _cache.Set(cacheKey, result.data, TimeSpan.FromSeconds(30));
                return result.data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch stock data from Dhan API");
        }

        return new List<DhanStockData>();
    }

    public async Task<DhanStockData?> GetStockBySymbolAsync(string symbol)
    {
        var stocks = await GetNifty50StocksAsync();
        return stocks.FirstOrDefault(s => s.Sym.Equals(symbol, StringComparison.OrdinalIgnoreCase));
    }
}
