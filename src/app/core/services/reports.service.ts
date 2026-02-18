import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ReportStats {
  totalDowntime: number;
  totalCost: number;
  safetyScore: number;
  totalAircraft: number;
  totalTasks: number;
}

@Injectable({ providedIn: 'root' })
export class ReportsService {
  private apiUrl = 'http://localhost:5000/api/reports';

  constructor(private http: HttpClient) {}

  getOverview(): Observable<ReportStats> {
    return this.http.get<ReportStats>(`${this.apiUrl}/overview`);
  }

  downloadFleetReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/fleet-summary`, { responseType: 'blob' });
  }
  getUpcomingTasks(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/upcoming`);
}
}