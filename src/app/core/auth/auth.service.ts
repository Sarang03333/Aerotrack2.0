import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface LoginResponse {
  access_token: string;
  expires_in: number;
  roles: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http.post<LoginResponse>(`${this.base}/api/auth/login`, { username, password });
  }

  saveToken(token: string) {
    localStorage.setItem('access_token', token);
  }

  logout() {
    localStorage.removeItem('access_token');
    location.href = '/login';
  }

  get token(): string | null { return localStorage.getItem('access_token'); }

  get roles(): string[] {
    const t = this.token;
    if (!t) return [];
    try {
      const payload = JSON.parse(atob(t.split('.')[1]));
      const role = payload['role'];
      if (!role) return [];
      return Array.isArray(role) ? role : [role];
    } catch { return []; }
  }

  isInRole(...allowed: string[]): boolean {
    const r = this.roles;
    return allowed.some(a => r.includes(a));
  }

  isAuthenticated(): boolean { return !!this.token; }
}