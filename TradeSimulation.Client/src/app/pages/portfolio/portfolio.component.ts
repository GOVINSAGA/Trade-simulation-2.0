import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PortfolioService } from '../../services/portfolio.service';
import { StockService } from '../../services/stock.service';
import { Holding, PortfolioSummary, Stock } from '../../models/interfaces';
import { TradeDialogComponent } from '../../components/trade-dialog/trade-dialog.component';

@Component({
  selector: 'app-portfolio',
  standalone: true,
  imports: [CommonModule, TradeDialogComponent],
  templateUrl: './portfolio.component.html',
  styleUrl: './portfolio.component.css'
})
export class PortfolioComponent implements OnInit, OnDestroy {
  holdings: Holding[] = [];
  summary: PortfolioSummary | null = null;
  loading = true;

  showTradeDialog = false;
  selectedStock: Stock | null = null;
  selectedHolding: Holding | null = null;

  private stocksCache: Stock[] = [];
  private refreshInterval: any;

  constructor(
    private portfolioService: PortfolioService,
    private stockService: StockService
  ) {}

  ngOnInit() {
    this.loadData();
    this.refreshInterval = setInterval(() => this.loadData(), 30000);
  }

  ngOnDestroy() {
    if (this.refreshInterval) clearInterval(this.refreshInterval);
  }

  loadData() {
    this.loading = true;

    this.portfolioService.getSummary().subscribe({
      next: (s) => this.summary = s,
      error: () => {}
    });

    this.portfolioService.getPortfolio().subscribe({
      next: (h) => {
        this.holdings = h;
        this.loading = false;
      },
      error: () => this.loading = false
    });

    this.stockService.getStocks().subscribe({
      next: (stocks) => this.stocksCache = stocks,
      error: () => {}
    });
  }

  openSellDialog(holding: Holding) {
    const stock = this.stocksCache.find(s => s.Sym === holding.Symbol);
    if (stock) {
      this.selectedStock = stock;
      this.selectedHolding = holding;
      this.showTradeDialog = true;
    }
  }

  onTraded() {
    this.showTradeDialog = false;
    this.selectedStock = null;
    this.selectedHolding = null;
    this.loadData();
  }
}
