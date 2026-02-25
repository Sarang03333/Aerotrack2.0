import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsService, ReportStats } from '../../core/services/reports.service';
@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styles: [`
    :host {
      display: block; background: #020617; min-height: 100vh;
      margin: -24px; padding: 24px;
    }
    .dashboard-container { width: 100%; max-width: 100%; }
    .stat-card, .report-card {
      background: rgba(15, 23, 42, 0.6) !important;
      backdrop-filter: blur(20px) saturate(180%);
      border: 1px solid rgba(59, 130, 246, 0.25) !important;
      border-radius: 20px;
      padding: 2rem !important; 
      overflow: hidden;
      box-shadow: 0 8px 32px 0 rgba(0, 0, 0, 0.4);
    }
    .col-md-5 .report-card {
      padding: 1.25rem 1.5rem !important;
    }

    .tiny { font-size: 0.7rem; color: #64748b; }

    .forecast-table tbody tr td {
      background: transparent !important;
      color: #cbd5e1 !important;
      border-bottom: 1px solid rgba(59, 130, 246, 0.1) !important;
      padding: 1.5rem 1rem !important;
    }
    
    .text-info { color: #60a5fa !important; }
  `]
})
export class ReportsComponent implements OnInit {
  stats: ReportStats | null = null;
  upcomingTasks: any[] = []; // New forecast array
  isGenerating = false;

  constructor(private reportsService: ReportsService) {}

  ngOnInit() {
    this.fetchOverview();
    this.fetchUpcoming();
  }

  fetchOverview() {
    this.reportsService.getOverview().subscribe({
      next: (data) => this.stats = data,
      error: (err) => console.error('Error fetching stats:', err)
    });
  }

  fetchUpcoming() {
    this.reportsService.getUpcomingTasks().subscribe({
      next: (data) => this.upcomingTasks = data,
      error: (err) => console.error('Error fetching forecast:', err)
    });
  }

  downloadCsv() {
    this.isGenerating = true;
    this.reportsService.downloadFleetReport().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `AeroTrack_Report_${new Date().toISOString().slice(0,10)}.csv`;
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