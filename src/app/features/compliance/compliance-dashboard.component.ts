import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-compliance-dashboard',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './compliance-dashboard.component.html'
})
export class ComplianceDashboardComponent implements OnInit {
  private apiUrl = 'http://localhost:5000/api';

  public darkOptions: ChartOptions = {
    responsive: true, maintainAspectRatio: false,
    plugins: { legend: { labels: { color: '#e2e8f0' } } },
    scales: {
      x: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } },
      y: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } }
    }
  };
  public pieOptions: ChartOptions = { ...this.darkOptions, scales: { x: { display: false }, y: { display: false } } };

  complianceData: ChartData<'doughnut'> = {
    labels: ['Compliant', 'Pending', 'Non-Compliant'],
    datasets: [{ data: [], backgroundColor: ['#22c55e','#eab308','#ef4444'], borderColor: '#1e293b' }]
  };
  
  auditsByMonthData: ChartData<'bar'> = {
    labels: ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'],
    datasets: [{ data: [], label: 'Audits Performed', backgroundColor: '#8b5cf6', hoverBackgroundColor: '#a78bfa' }]
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    // 1. Fetch Aircraft for Compliance Pie
    this.http.get<any[]>(`${this.apiUrl}/aircraft`).subscribe({
      next: (list) => {
        const compMap = new Map<string, number>();
        list.forEach(a => {
          const s = a.complianceStatus || 'Pending';
          compMap.set(s, (compMap.get(s) || 0) + 1);
        });

        this.complianceData = { 
          labels: Array.from(compMap.keys()),
          datasets: [{ 
            ...this.complianceData.datasets[0], 
            data: Array.from(compMap.values()),
            backgroundColor: Array.from(compMap.keys()).map(k => {
                if(k === 'Compliant') return '#22c55e';
                if(k === 'Non-Compliant') return '#ef4444';
                return '#eab308';
            })
          }] 
        };
      },
      error: (err) => console.error('Failed to load aircraft compliance', err)
    });

    // 2. Fetch Audit Logs for Timeline
    this.http.get<any[]>(`${this.apiUrl}/compliance/audits`).subscribe({
      next: (list) => {
        const months = new Array(12).fill(0);
        list.forEach(a => {
          if (a.date) {
            const d = new Date(a.date);
            if (!isNaN(d.getMonth())) months[d.getMonth()]++;
          }
        });

        this.auditsByMonthData = {
          ...this.auditsByMonthData,
          datasets: [{ ...this.auditsByMonthData.datasets[0], data: months }]
        };
      },
      error: (err) => console.error('Failed to load audits', err)
    });
  }
}