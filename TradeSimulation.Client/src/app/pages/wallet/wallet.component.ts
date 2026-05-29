import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WalletService } from '../../services/wallet.service';
import { PortfolioService } from '../../services/portfolio.service';
import { Wallet, Transaction } from '../../models/interfaces';

@Component({
  selector: 'app-wallet',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './wallet.component.html',
  styleUrl: './wallet.component.css'
})
export class WalletComponent implements OnInit {
  wallet: Wallet | null = null;
  transactions: Transaction[] = [];
  depositAmount = 0;
  withdrawAmount = 0;
  loading = true;
  actionLoading = false;
  error = '';
  success = '';

  constructor(
    private walletService: WalletService,
    private portfolioService: PortfolioService
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.walletService.getWallet().subscribe({
      next: (w) => {
        this.wallet = w;
        this.loading = false;
      },
      error: () => this.loading = false
    });

    this.portfolioService.getTransactions().subscribe({
      next: (t) => this.transactions = t,
      error: () => {}
    });
  }

  deposit() {
    if (this.depositAmount <= 0) return;
    this.actionLoading = true;
    this.error = '';
    this.success = '';

    this.walletService.deposit(this.depositAmount).subscribe({
      next: (w) => {
        this.wallet = w;
        this.success = `₹${this.depositAmount.toLocaleString()} deposited successfully`;
        this.depositAmount = 0;
        this.actionLoading = false;
        this.loadData();
      },
      error: (err) => {
        this.error = err.error?.message || 'Deposit failed';
        this.actionLoading = false;
      }
    });
  }

  withdraw() {
    if (this.withdrawAmount <= 0) return;
    this.actionLoading = true;
    this.error = '';
    this.success = '';

    this.walletService.withdraw(this.withdrawAmount).subscribe({
      next: (w) => {
        this.wallet = w;
        this.success = `₹${this.withdrawAmount.toLocaleString()} withdrawn successfully`;
        this.withdrawAmount = 0;
        this.actionLoading = false;
        this.loadData();
      },
      error: (err) => {
        this.error = err.error?.message || 'Withdrawal failed';
        this.actionLoading = false;
      }
    });
  }

  getTransactionTypeLabel(type: number): string {
    switch (type) {
      case 0: return 'DEPOSIT';
      case 1: return 'WITHDRAW';
      case 2: return 'BUY';
      case 3: return 'SELL';
      default: return 'UNKNOWN';
    }
  }

  getTransactionTypeClass(type: number): string {
    switch (type) {
      case 0: return 'type-deposit';
      case 1: return 'type-withdraw';
      case 2: return 'type-buy';
      case 3: return 'type-sell';
      default: return '';
    }
  }
}
