namespace TradeSimulation.API.Models;

public record DepositRequest(decimal Amount);
public record WithdrawRequest(decimal Amount);

public record BuyRequest(string Symbol, string DisplayName, int Quantity, decimal PricePerShare, string Sector);
public record SellRequest(string Symbol, int Quantity, decimal PricePerShare);

public record PortfolioHolding(
    string Symbol,
    string DisplayName,
    int Quantity,
    decimal AvgBuyPrice,
    decimal CurrentPrice,
    decimal InvestedValue,
    decimal CurrentValue,
    decimal PnL,
    decimal PnLPercent,
    string Sector
);

public record PortfolioSummary(
    decimal TotalInvested,
    decimal CurrentValue,
    decimal TotalPnL,
    decimal TotalPnLPercent,
    int TotalHoldings,
    decimal WalletBalance
);

public record DhanStockData(
    string Sym,
    string DispSym,
    string Exch,
    decimal Ltp,
    decimal Mcap,
    decimal High1Yr,
    decimal Low1Yr,
    decimal Pe,
    decimal ROCE,
    decimal PricePerchng1mon,
    decimal PricePerchng1year,
    string Sector,
    decimal PPerchange,
    decimal Pchange,
    long Volume,
    DhanAnalystRating? AnalystRating
);

public record DhanAnalystRating(
    int Buy,
    decimal BuyPer,
    int Hold,
    decimal HoldPer,
    string Rating,
    int Sell,
    decimal SellPer,
    int Total
);

public record DhanApiResponse(
    int code,
    int tot_rec,
    List<DhanStockData> data
);

public record DhanApiRequest(DhanRequestData data);

public record DhanRequestData(
    string sort,
    string sorder,
    int count,
    List<DhanParam> @params,
    List<string> fields,
    int pgno
);

public record DhanParam(string field, string op, string val);
