import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { WalletService } from '../../services/wallet.service';
import { AuthService } from '../../services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    @if (isLoggedIn) {
      <nav class="navbar">
        <div class="nav-left">
          <a routerLink="/" class="logo">
            <span class="logo-icon">📈</span>
            <span class="logo-text">TradeSimulator</span>
            <span class="logo-badge">PAPER TRADING</span>
          </a>
        </div>
        <div class="nav-center">
          <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link">◉ Dashboard</a>
          <a routerLink="/market" routerLinkActive="active" class="nav-link">◈ Market</a>
          <a routerLink="/portfolio" routerLinkActive="active" class="nav-link">◆ Portfolio</a>
          <a routerLink="/wallet" routerLinkActive="active" class="nav-link">◇ Wallet</a>
        </div>
        <div class="nav-right">
          <div class="user-info">
            <span class="user-name">{{ userName }}</span>
            <span class="balance-label">BALANCE</span>
            <span class="balance-value">₹{{ balance | number:'1.2-2' }}</span>
          </div>
          <button class="logout-btn" (click)="logout()">Logout</button>
        </div>
      </nav>
    }
  `,
  styles: [`
    .navbar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 24px;
      height: 64px;
      background: #0a0a0a;
      border-bottom: 1px solid #1a1a1a;
    }
    .nav-left { display: flex; align-items: center; }
    .logo { display: flex; align-items: center; gap: 10px; text-decoration: none; }
    .logo-icon { font-size: 24px; }
    .logo-text { font-family: 'Outfit', sans-serif; font-size: 18px; font-weight: 700; color: #fff; }
    .logo-badge {
      font-family: 'Space Mono', monospace; font-size: 9px; font-weight: 700;
      color: #000; background: #D4AF37; padding: 3px 8px; border-radius: 4px;
      letter-spacing: 1px;
    }
    .nav-center { display: flex; gap: 8px; }
    .nav-link {
      font-family: 'Outfit', sans-serif; font-size: 14px; font-weight: 500;
      color: #888; text-decoration: none; padding: 8px 16px; border-radius: 6px;
      transition: all 0.15s ease;
    }
    .nav-link:hover { color: #ccc; background: #1a1a1a; }
    .nav-link.active { color: #D4AF37; background: rgba(212, 175, 55, 0.08); }
    .nav-right { display: flex; align-items: center; gap: 16px; }
    .user-info { display: flex; flex-direction: column; align-items: flex-end; gap: 1px; }
    .user-name { font-family: 'Outfit', sans-serif; font-size: 12px; color: #888; }
    .balance-label { font-family: 'Space Mono', monospace; font-size: 9px; color: #666; letter-spacing: 1px; }
    .balance-value { font-family: 'Space Mono', monospace; font-size: 15px; font-weight: 700; color: #D4AF37; }
    .logout-btn {
      font-family: 'Outfit', sans-serif; font-size: 12px; font-weight: 500;
      color: #888; background: #1a1a1a; border: 1px solid #2a2a2a;
      padding: 6px 14px; border-radius: 6px; cursor: pointer;
      transition: all 0.15s ease;
    }
    .logout-btn:hover { color: #E74C3C; border-color: rgba(231, 76, 60, 0.3); background: rgba(231, 76, 60, 0.08); }
  `]
})
export class NavbarComponent implements OnInit, OnDestroy {
  balance = 0;
  isLoggedIn = false;
  userName = '';
  private sub?: Subscription;

  constructor(
    private walletService: WalletService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.sub = this.authService.isLoggedIn$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      if (loggedIn) {
        this.userName = this.authService.getCurrentUser()?.FullName || '';
        this.loadBalance();
      }
    });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }

  loadBalance() {
    this.walletService.getWallet().subscribe({
      next: (w) => this.balance = w.Balance,
      error: () => {}
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
