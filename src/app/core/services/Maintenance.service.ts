import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MaintenanceService {
  // Ensure the port matches your backend (5000 or 5001)
  private apiUrl = 'http://localhost:5000/api/maintenance/tasks';

  constructor(private http: HttpClient) {}

  getTasks(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getTask(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createTask(dto: any): Observable<any> {
    return this.http.post(this.apiUrl, dto);
  }

  updateTask(id: string, dto: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, dto);
  }

  // NEW: Matches [HttpPut("tasks/{id}/complete")] in your controller
  completeTask(id: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/complete`, {});
  }

  // NEW: Matches [HttpDelete("tasks/{id}")] in your controller
  deleteTask(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}