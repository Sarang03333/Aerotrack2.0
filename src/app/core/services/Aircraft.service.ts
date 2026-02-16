import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AircraftService {
  private apiUrl = 'http://localhost:5000/api/aircraft';

  constructor(private http: HttpClient) { }

  getAircrafts(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // Matches the call in ngOnInit
  getAircraft(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  // Matches the call in save() for new entries
  createAircraft(aircraft: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, aircraft);
  }

  // Matches the call in save() for updates
  updateAircraft(id: string, aircraft: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, aircraft);
  }
}