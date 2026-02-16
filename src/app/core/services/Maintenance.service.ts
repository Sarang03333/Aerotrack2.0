import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MaintenanceService {
  private apiUrl = 'http://localhost:5000/api/maintenance/tasks';

  constructor(private http: HttpClient) {}

  // Calls the [HttpGet] in MaintenanceController
  getTasks(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // Calls the [HttpGet("{id}")] in MaintenanceController
  getTask(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  // Calls the [HttpPost] and triggers DTO Validation
  createTask(dto: any): Observable<any> {
    return this.http.post(this.apiUrl, dto);
  }

  // Calls the [HttpPut]
  updateTask(id: string, dto: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, dto);
  }
}