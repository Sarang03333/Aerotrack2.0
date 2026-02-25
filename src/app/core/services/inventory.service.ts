import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private apiUrl = 'https://localhost:5001/api/Inventory/parts';

  constructor(private http: HttpClient) { }

  getParts(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getPart(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createPart(part: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, part);
  }

  updatePart(id: string, part: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, part);
  }

  deletePart(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  replenishPart(id: string): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/replenish`, {});
}
}