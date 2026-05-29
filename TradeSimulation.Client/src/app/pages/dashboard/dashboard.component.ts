import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { StockService } from '../../services/stock.service';
import { PortfolioService } from '../../services/portfolio.service';
import { WalletService } from '../../services/wallet.service';
import { Stock, PortfolioSummary, Wallet } from '../../models/interfaces';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy {
  wallet: Wallet | null = null;
  summary: PortfolioSummary | null = null;
  stocks: Stock[] = [];
  topGainers: Stock[] = [];
  topLosers: Stock[] = [];
  loading = true;
  private refreshInterval: any;

  constructor(
    private stockService: StockService,
    private portfolioService: PortfolioService,
    private walletService: WalletService
  ) {}

  ngOnInit() {
    this.loadData();
    this.refreshInterval = setInterval(() => this.loadData(), 60000);
  }

  ngOnDestroy() {
    if (this.refreshInterval) clearInterval(this.refreshInterval);
  }

  loadData() {
    this.loading = true;

    this.walletService.getWallet().subscribe({
      next: (w) => this.wallet = w,
      error: () => {}
    });

    this.portfolioService.getSummary().subscribe({
      next: (s) => this.summary = s,
      error: () => {}
    });

    this.stockService.getStocks().subscribe({
      next: (stocks) => {
        this.stocks = stocks;
        const sorted = [...stocks].sort((a, b) => b.PPerchange - a.PPerchange);
        this.topGainers = sorted.slice(0, 5);
        this.topLosers = sorted.slice(-5).reverse();
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}
