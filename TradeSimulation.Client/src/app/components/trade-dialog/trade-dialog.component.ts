import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Stock } from '../../models/interfaces';
import { TradeService } from '../../services/trade.service';

@Component({
  selector: 'app-trade-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './trade-dialog.component.html',
  styleUrl: './trade-dialog.component.css'
})
export class TradeDialogComponent {
  @Input() stock: Stock | null = null;
  @Input() mode: 'buy' | 'sell' = 'buy';
  @Input() maxQuantity = 0;
  @Output() close = new EventEmitter<void>();
  @Output() traded = new EventEmitter<void>();

  quantity = 1;
  loading = false;
  error = '';

  constructor(private tradeService: TradeService) {}

  get totalCost(): number {
    return this.quantity * (this.stock?.Ltp || 0);
  }

  executeTrade() {
    if (!this.stock || this.quantity <= 0) return;
    this.loading = true;
    this.error = '';

    if (this.mode === 'buy') {
      this.tradeService.buy(
        this.stock.Sym,
        this.stock.DispSym,
        this.quantity,
        this.stock.Ltp,
        this.stock.Sector
      ).subscribe({
        next: () => {
          this.loading = false;
          this.traded.emit();
        },
        error: (err) => {
          this.loading = false;
          this.error = err.error?.message || 'Trade failed';
        }
      });
    } else {
      this.tradeService.sell(
        this.stock.Sym,
        this.quantity,
        this.stock.Ltp
      ).subscribe({
        next: () => {
          this.loading = false;
          this.traded.emit();
        },
        error: (err) => {
          this.loading = false;
          this.error = err.error?.message || 'Trade failed';
        }
      });
    }
  }
}
