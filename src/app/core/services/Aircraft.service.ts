import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AircraftService {
  private apiUrl = 'http://localhost:5000/api/aircraft';

  constructor(private http: HttpClient) {}

  getAircrafts(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getAircraft(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createAircraft(dto: any): Observable<any> {
    return this.http.post(this.apiUrl, dto);
  }

  updateAircraft(id: string, dto: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, dto);
  }

  // Added deleteAircraft to resolve ts(2551)
  deleteAircraft(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}