import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface ReportStats {
  totalDowntime: number;
  totalCost: number;
  safetyScore: number;
  totalAircraft: number;
  totalTasks: number;
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styles: [`
    .stat-card {
      background: rgba(30, 41, 59, 0.5); /* Semi-transparent slate */
      border: 1px solid #334155;
      border-radius: 12px;
      padding: 1.5rem;
    }
    .report-action-card {
      background: #1e293b;
      border: 1px solid #334155;
      border-radius: 12px;
      transition: all 0.2s;
    }
    .report-action-card:hover {
      border-color: #3b82f6;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0,0,0,0.3);
    }
  `]
})
export class ReportsComponent implements OnInit {
  stats: ReportStats | null = null;
  isGenerating = false;
  private apiUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchOverview();
  }

  fetchOverview() {
    this.http.get<ReportStats>(`${this.apiUrl}/reports/overview`).subscribe({
      next: (data) => this.stats = data,
      error: (err) => console.error('Error fetching report stats:', err)
    });
  }

  downloadCsv() {
    this.isGenerating = true;
    this.http.get(`${this.apiUrl}/reports/fleet-summary`, { responseType: 'blob' }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `AeroTrack_Combined_Report_${new Date().toISOString().slice(0,10)}.csv`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.isGenerating = false;
      },
      error: () => {
        alert('Download failed.');
        this.isGenerating = false;
      }
    });
  }
}