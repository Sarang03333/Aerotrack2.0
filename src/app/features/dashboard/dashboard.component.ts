import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { StatCardComponent } from "../../shared/components/stat-card/stat-card.component";
import { BaseChartDirective } from "ng2-charts";
import { ChartConfiguration, ChartOptions } from "chart.js";

@Component({
  selector: "app-dashboard",
  standalone: true,
  imports: [CommonModule, StatCardComponent, BaseChartDirective],
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.css"],
})
export class DashboardComponent implements OnInit {
  private apiUrl = "http://localhost:5000/api";

  // --- 1. KPI METRICS ---
  metrics = {
    totalAircraft: 0,
    openTasks: 0,
    completedTasks: 0,
    nonCompliant: 0
  };

  // --- 2. CHART OPTIONS (Dark Theme) ---
  public darkOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { labels: { color: '#e2e8f0' } } },
    scales: {
      x: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } },
      y: { ticks: { color: '#94a3b8' }, grid: { color: 'rgba(255,255,255,0.05)' } }
    }
  };
  public pieOptions: ChartOptions = {
    ...this.darkOptions,
    scales: { x: { display: false }, y: { display: false } }
  };

  // --- 3. CHART DATA VARIABLES ---
  
  // Aircraft Chart (Doughnut) - By Category
  aircraftData: ChartConfiguration<'doughnut'>['data'] = {
    labels: [], 
    datasets: [{ data: [], backgroundColor: ['#3b82f6', '#eab308', '#10b981'], borderColor: '#1e293b' }]
  };

  // Maintenance Chart (Bar) - Tasks per Aircraft
  maintenanceData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [{ data: [], label: 'Active Tasks', backgroundColor: '#3b82f6' }]
  };

  // Inventory Chart (Bar) - Top Parts Quantity
  inventoryData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [{ data: [], label: 'Stock Level', backgroundColor: '#8b5cf6' }]
  };

  // Compliance Chart (Pie) - Status Breakdown
  complianceData: ChartConfiguration<'pie'>['data'] = {
    labels: ['Compliant', 'Pending', 'Non-Compliant'],
    datasets: [{ data: [], backgroundColor: ['#22c55e', '#eab308', '#ef4444'], borderColor: '#1e293b' }]
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadRealData();
  }

  loadRealData() {
    // A. FETCH AIRCRAFT (Populates Metrics, Aircraft Chart, Compliance Chart)
    this.http.get<any[]>(`${this.apiUrl}/aircraft`).subscribe(list => {
      this.metrics.totalAircraft = list.length;

      // 1. Compliance Logic
      let comp = 0, pend = 0, non = 0;
      // 2. Category Logic
      const catMap = new Map<string, number>();

      list.forEach(a => {
        // Count Compliance
        if (a.complianceStatus === 'Compliant') comp++;
        else if (a.complianceStatus === 'Pending') pend++;
        else non++;

        // Count Category
        const c = a.category || 'Unknown';
        catMap.set(c, (catMap.get(c) || 0) + 1);
      });

      this.metrics.nonCompliant = non;

      // Update Compliance Chart
      this.complianceData = {
        ...this.complianceData,
        datasets: [{ ...this.complianceData.datasets[0], data: [comp, pend, non] }]
      };

      // Update Aircraft Category Chart
      this.aircraftData = {
        labels: Array.from(catMap.keys()),
        datasets: [{ ...this.aircraftData.datasets[0], data: Array.from(catMap.values()) }]
      };
    });

    // B. FETCH MAINTENANCE (Populates Metrics, Maintenance Chart)
    this.http.get<any[]>(`${this.apiUrl}/maintenance/tasks`).subscribe(list => {
      let open = 0, closed = 0;
      const acMap = new Map<string, number>();

      list.forEach(t => {
        if (t.status === 'COMPLETED') closed++;
        else {
          open++;
          // Count active tasks per aircraft for the Bar Chart
          if (t.aircraftId) {
            acMap.set(t.aircraftId, (acMap.get(t.aircraftId) || 0) + 1);
          }
        }
      });

      this.metrics.openTasks = open;
      this.metrics.completedTasks = closed;

      // Top 5 Aircraft by Workload
      const topAc = Array.from(acMap.entries())
        .sort((a, b) => b[1] - a[1])
        .slice(0, 5);

      this.maintenanceData = {
        labels: topAc.map(e => e[0]),
        datasets: [{ ...this.maintenanceData.datasets[0], data: topAc.map(e => e[1]) }]
      };
    });

    // C. FETCH INVENTORY (Populates Inventory Chart)
    this.http.get<any[]>(`${this.apiUrl}/inventory/parts`).subscribe(list => {
      // Show top 6 parts by quantity
      const sorted = list.sort((a, b) => b.quantityAvailable - a.quantityAvailable).slice(0, 6);

      this.inventoryData = {
        labels: sorted.map(p => p.name),
        datasets: [{ ...this.inventoryData.datasets[0], data: sorted.map(p => p.quantityAvailable) }]
      };
    });
  }
}