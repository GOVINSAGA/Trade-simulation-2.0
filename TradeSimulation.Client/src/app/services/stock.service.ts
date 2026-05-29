import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Stock } from '../models/interfaces';

@Injectable({ providedIn: 'root' })
export class StockService {
  private baseUrl = '/api/stocks';

  constructor(private http: HttpClient) {}

  getStocks(sort = 'Mcap', order = 'desc'): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.baseUrl}?sort=${sort}&order=${order}`);
  }

  getStock(symbol: string): Observable<Stock> {
    return this.http.get<Stock>(`${this.baseUrl}/${symbol}`);
  }
}
