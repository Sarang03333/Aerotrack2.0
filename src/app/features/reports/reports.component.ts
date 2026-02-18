import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsService, ReportStats } from '../../core/services/reports.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styles: [`
    /* Wrapper to kill white space and match theme */
    :host {
      display: block;
      background: #0f172a; 
      min-height: 100vh;
      margin: -24px; /* Counteracts standard page padding */
      padding: 24px;
    }

    .dashboard-container { 
      width: 100%; 
      max-width: 100%; /* Ensures full-width span */
    }

    .stat-card {
      background: rgba(30, 41, 59, 0.7);
      backdrop-filter: blur(10px);
      border: 1px solid rgba(255, 255, 255, 0.1);
      border-radius: 16px;
      padding: 1.5rem;
    }

    .report-card {
      background: #1e293b;
      border: 1px solid #334155;
      border-radius: 16px;
    }

    .text-gradient {
      background: linear-gradient(to right, #60a5fa, #a78bfa);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }
  `]
})
export class ReportsComponent implements OnInit {
  stats: ReportStats | null = null;
  isGenerating = false;

  // Injecting the new ReportsService
  constructor(private reportsService: ReportsService) {}

  ngOnInit() {
    this.fetchOverview();
  }

  fetchOverview() {
    // Calling the service instead of direct HttpClient
    this.reportsService.getOverview().subscribe({
      next: (data) => this.stats = data,
      error: (err) => console.error('Error fetching report stats:', err)
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