export interface Stock {
  Sym: string;
  DispSym: string;
  Exch: string;
  Ltp: number;
  Mcap: number;
  High1Yr: number;
  Low1Yr: number;
  Pe: number;
  ROCE: number;
  PricePerchng1mon: number;
  PricePerchng1year: number;
  Sector: string;
  PPerchange: number;
  Pchange: number;
  Volume: number;
  AnalystRating: {
    Buy: number;
    BuyPer: number;
    Hold: number;
    HoldPer: number;
    Rating: string;
    Sell: number;
    SellPer: number;
    Total: number;
  } | null;
}

export interface Wallet {
  Id: number;
  Balance: number;
  CreatedAt: string;
  UpdatedAt: string;
}

export interface Trade {
  Id: number;
  Symbol: string;
  DisplayName: string;
  Quantity: number;
  PricePerShare: number;
  TotalAmount: number;
  Type: number;
  Sector: string;
  TradedAt: string;
}

export interface Holding {
  Symbol: string;
  DisplayName: string;
  Quantity: number;
  AvgBuyPrice: number;
  CurrentPrice: number;
  InvestedValue: number;
  CurrentValue: number;
  PnL: number;
  PnLPercent: number;
  Sector: string;
}

export interface PortfolioSummary {
  TotalInvested: number;
  CurrentValue: number;
  TotalPnL: number;
  TotalPnLPercent: number;
  TotalHoldings: number;
  WalletBalance: number;
}

export interface Transaction {
  Id: number;
  Type: number;
  Amount: number;
  Description: string;
  CreatedAt: string;
}

export interface AuthResponse {
  Token: string;
  FullName: string;
  Email: string;
}

export interface RegisterRequest {
  FullName: string;
  Email: string;
  Password: string;
}

export interface LoginRequest {
  Email: string;
  Password: string;
}
