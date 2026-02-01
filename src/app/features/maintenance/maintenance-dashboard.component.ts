import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BaseChartDirective } from "ng2-charts";
import { ChartData, ChartOptions } from "chart.js";

@Component({
  selector: "app-maintenance-dashboard",
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: "./maintenance-dashboard.component.html",
})
export class MaintenanceDashboardComponent implements OnInit {
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

  statusData: ChartData<"doughnut"> = {
    labels: ["Pending", "In Progress", "Completed"],
    datasets: [{ data: [], backgroundColor: ["#94a3b8", "#3b82f6", "#22c55e"], borderColor: '#1e293b' }]
  };
  
  tasksPerDayData: ChartData<"line"> = {
    labels: [],
    datasets: [{ data: [], label: "Tasks", backgroundColor: "rgba(59,130,246,0.2)", borderColor: "#3b82f6", fill: true, tension: 0.4 }]
  };
  
  topAircraftData: ChartData<"bar"> = {
    labels: [],
    datasets: [{ data: [], label: "Tasks", backgroundColor: "#eab308", hoverBackgroundColor: "#facc15" }]
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any[]>(`${this.apiUrl}/maintenance/tasks`).subscribe({
      next: (list) => {
        const statusMap = new Map<string, number>();
        const dateMap = new Map<string, number>();
        const acMap = new Map<string, number>();

        list.forEach(t => {
          // 1. Status
          const s = t.status || 'UNKNOWN';
          statusMap.set(s, (statusMap.get(s) || 0) + 1);

          // 2. Schedule Date (Workload)
          if (t.scheduledDate) {
            // Take YYYY-MM-DD part only
            const dateStr = t.scheduledDate.toString().split('T')[0];
            dateMap.set(dateStr, (dateMap.get(dateStr) || 0) + 1);
          }

          // 3. Aircraft ID
          if (t.aircraftId) {
            acMap.set(t.aircraftId, (acMap.get(t.aircraftId) || 0) + 1);
          }
        });

        // Update Status Chart
        this.statusData = {
          labels: Array.from(statusMap.keys()),
          datasets: [{ ...this.statusData.datasets[0], data: Array.from(statusMap.values()) }]
        };

        // Update Timeline (Sort by date)
        const sortedDates = Array.from(dateMap.keys()).sort();
        this.tasksPerDayData = {
          labels: sortedDates,
          datasets: [{ ...this.tasksPerDayData.datasets[0], data: sortedDates.map(d => dateMap.get(d) || 0) }]
        };

        // Update Top Aircraft (Top 5)
        const sortedAc = Array.from(acMap.entries()).sort((a, b) => b[1] - a[1]).slice(0, 5);
        this.topAircraftData = {
          labels: sortedAc.map(e => e[0]),
          datasets: [{ ...this.topAircraftData.datasets[0], data: sortedAc.map(e => e[1]) }]
        };
      },
      error: (err) => console.error('Failed to load maintenance tasks', err)
    });
  }
}