import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Trade } from '../models/interfaces';

@Injectable({ providedIn: 'root' })
export class TradeService {
  private baseUrl = '/api/trades';

  constructor(private http: HttpClient) {}

  buy(symbol: string, displayName: string, quantity: number, pricePerShare: number, sector: string): Observable<Trade> {
    return this.http.post<Trade>(`${this.baseUrl}/buy`, {
      Symbol: symbol, DisplayName: displayName, Quantity: quantity, PricePerShare: pricePerShare, Sector: sector
    });
  }

  sell(symbol: string, quantity: number, pricePerShare: number): Observable<Trade> {
    return this.http.post<Trade>(`${this.baseUrl}/sell`, {
      Symbol: symbol, Quantity: quantity, PricePerShare: pricePerShare
    });
  }

  getHistory(): Observable<Trade[]> {
    return this.http.get<Trade[]>(`${this.baseUrl}/history`);
  }
}
