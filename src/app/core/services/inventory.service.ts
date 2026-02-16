import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private apiUrl = 'http://localhost:5000/api/inventory';

  constructor(private http: HttpClient) { }

  getPart(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createPart(part: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, part);
  }

  updatePart(id: string, part: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, part);
  }
}