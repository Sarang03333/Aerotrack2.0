import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ComplianceService {
  private apiUrl = 'http://localhost:5000/api/compliance/audits';

  constructor(private http: HttpClient) { }

  getAudits(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getAudit(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createAudit(audit: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, audit);
  }

  updateAudit(id: string, audit: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, audit);
  }
}