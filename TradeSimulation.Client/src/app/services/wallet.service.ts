import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Wallet } from '../models/interfaces';

@Injectable({ providedIn: 'root' })
export class WalletService {
  private baseUrl = '/api/wallet';

  constructor(private http: HttpClient) {}

  getWallet(): Observable<Wallet> {
    return this.http.get<Wallet>(this.baseUrl);
  }

  deposit(amount: number): Observable<Wallet> {
    return this.http.post<Wallet>(`${this.baseUrl}/deposit`, { Amount: amount });
  }

  withdraw(amount: number): Observable<Wallet> {
    return this.http.post<Wallet>(`${this.baseUrl}/withdraw`, { Amount: amount });
  }
}
