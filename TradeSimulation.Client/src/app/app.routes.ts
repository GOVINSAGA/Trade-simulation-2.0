import { Routes } from '@angular/router';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { MarketComponent } from './pages/market/market.component';
import { PortfolioComponent } from './pages/portfolio/portfolio.component';
import { WalletComponent } from './pages/wallet/wallet.component';
import { LoginComponent } from './pages/login/login.component';
import { SignupComponent } from './pages/signup/signup.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: '', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'market', component: MarketComponent, canActivate: [authGuard] },
  { path: 'portfolio', component: PortfolioComponent, canActivate: [authGuard] },
  { path: 'wallet', component: WalletComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
