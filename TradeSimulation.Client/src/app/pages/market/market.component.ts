import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService } from '../../services/stock.service';
import { Stock } from '../../models/interfaces';
import { TradeDialogComponent } from '../../components/trade-dialog/trade-dialog.component';

@Component({
  selector: 'app-market',
  standalone: true,
  imports: [CommonModule, FormsModule, TradeDialogComponent],
  templateUrl: './market.component.html',
  styleUrl: './market.component.css'
})
export class MarketComponent implements OnInit, OnDestroy {
  stocks: Stock[] = [];
  filteredStocks: Stock[] = [];
  searchQuery = '';
  sortField = 'Mcap';
  sortOrder = 'desc';
  loading = true;
  selectedStock: Stock | null = null;
  showTradeDialog = false;
  lastUpdated = '';
  countdown = 30;
  private refreshInterval: any;
  private countdownInterval: any;

  constructor(private stockService: StockService) {}

  ngOnInit() {
    this.loadStocks();
    this.startAutoRefresh();
  }

  ngOnDestroy() {
    this.stopAutoRefresh();
  }

  loadStocks() {
    this.loading = true;
    this.stockService.getStocks(this.sortField, this.sortOrder).subscribe({
      next: (stocks) => {
        this.stocks = stocks;
        this.applyFilter();
        this.lastUpdated = new Date().toLocaleTimeString();
        this.loading = false;
        this.countdown = 30;
      },
      error: () => this.loading = false
    });
  }

  private startAutoRefresh() {
    this.refreshInterval = setInterval(() => this.loadStocks(), 30000);
    this.countdownInterval = setInterval(() => {
      if (this.countdown > 0) this.countdown--;
    }, 1000);
  }

  private stopAutoRefresh() {
    if (this.refreshInterval) clearInterval(this.refreshInterval);
    if (this.countdownInterval) clearInterval(this.countdownInterval);
  }

  applyFilter() {
    if (!this.searchQuery.trim()) {
      this.filteredStocks = this.stocks;
    } else {
      const q = this.searchQuery.toLowerCase();
      this.filteredStocks = this.stocks.filter(s =>
        s.Sym.toLowerCase().includes(q) ||
        s.DispSym.toLowerCase().includes(q) ||
        s.Sector.toLowerCase().includes(q)
      );
    }
  }

  sortBy(field: string) {
    if (this.sortField === field) {
      this.sortOrder = this.sortOrder === 'desc' ? 'asc' : 'desc';
    } else {
      this.sortField = field;
      this.sortOrder = 'desc';
    }
    this.loadStocks();
  }

  getSortIcon(field: string): string {
    if (this.sortField !== field) return '⇅';
    return this.sortOrder === 'desc' ? '▼' : '▲';
  }

  openBuyDialog(stock: Stock) {
    this.selectedStock = stock;
    this.showTradeDialog = true;
  }

  onTraded() {
    this.showTradeDialog = false;
    this.selectedStock = null;
  }

  formatMcap(mcap: number): string {
    if (mcap >= 100000) return (mcap / 100000).toFixed(1) + 'L Cr';
    if (mcap >= 1000) return (mcap / 1000).toFixed(1) + 'K Cr';
    return mcap.toFixed(0) + ' Cr';
  }

  formatVolume(vol: number): string {
    if (vol >= 10000000) return (vol / 10000000).toFixed(2) + ' Cr';
    if (vol >= 100000) return (vol / 100000).toFixed(2) + ' L';
    if (vol >= 1000) return (vol / 1000).toFixed(1) + 'K';
    return vol.toString();
  }
}
