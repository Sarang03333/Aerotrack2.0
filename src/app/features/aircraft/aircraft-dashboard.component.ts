import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BaseChartDirective } from "ng2-charts";
import { ChartData, ChartOptions } from "chart.js";

@Component({
  selector: "app-aircraft-dashboard",
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: "./aircraft-dashboard.component.html",
})
export class AircraftDashboardComponent implements OnInit {
  private apiUrl = 'http://localhost:5000/api';

  // --- DARK THEME OPTIONS ---
  public darkOptions: ChartOptions = {
    responsive: true, maintainAspectRatio: false,
    plugins: { legend: { labels: { color: '#e2e8f0' } } },
    scales: {
      x: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } },
      y: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } }
    }
  };
  public pieOptions: ChartOptions = { ...this.darkOptions, scales: { x: { display: false }, y: { display: false } } };

  categoryData: ChartData<"doughnut"> = {
    labels: ["Commercial", "Defense", "Cargo"],
    datasets: [{ data: [], backgroundColor: ["#3b82f6", "#eab308", "#10b981"], borderColor: '#1e293b' }]
  };
  
  complianceData: ChartData<"pie"> = {
    labels: ["Compliant", "Pending", "Non-Compliant"],
    datasets: [{ data: [], backgroundColor: ["#22c55e", "#f59e0b", "#ef4444"], borderColor: '#1e293b' }]
  };

  servicesByMonthData: ChartData<"bar"> = {
    labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
    datasets: [{ data: [], label: "Last Service (Count)", backgroundColor: "#6366f1", hoverBackgroundColor: "#818cf8" }]
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any[]>(`${this.apiUrl}/aircraft`).subscribe({
      next: (list) => {
        const catMap = new Map<string, number>();
        const compMap = new Map<string, number>();
        const months = new Array(12).fill(0);

        list.forEach(a => {
          // 1. Category Count
          const c = a.category || 'Other';
          catMap.set(c, (catMap.get(c) || 0) + 1);

          // 2. Compliance Count
          const s = a.complianceStatus || 'Pending';
          compMap.set(s, (compMap.get(s) || 0) + 1);

          // 3. Service History (Last Service Date)
          if (a.lastServiceDate) {
            const d = new Date(a.lastServiceDate);
            if (!isNaN(d.getMonth())) months[d.getMonth()]++;
          }
        });

        // Update Charts
        this.categoryData = {
          labels: Array.from(catMap.keys()),
          datasets: [{ ...this.categoryData.datasets[0], data: Array.from(catMap.values()) }]
        };

        this.complianceData = {
          labels: Array.from(compMap.keys()),
          datasets: [{ 
            ...this.complianceData.datasets[0], 
            data: Array.from(compMap.values()),
            backgroundColor: Array.from(compMap.keys()).map(k => this.getColorForStatus(k))
          }]
        };

        this.servicesByMonthData = {
          ...this.servicesByMonthData,
          datasets: [{ ...this.servicesByMonthData.datasets[0], data: months }]
        };
      },
      error: (err) => console.error('Failed to load aircraft data', err)
    });
  }

  private getColorForStatus(status: string): string {
    switch (status?.toLowerCase()) {
      case 'compliant': return '#22c55e';
      case 'non-compliant': return '#ef4444';
      default: return '#f59e0b';
    }
  }
}