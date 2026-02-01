import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-inventory-dashboard',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './inventory-dashboard.component.html'
})
export class InventoryDashboardComponent implements OnInit {
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

  stockHealthData: ChartData<'doughnut'> = {
    labels: ['Low Stock', 'Healthy'],
    datasets: [{ data: [], backgroundColor: ['#ef4444', '#22c55e'], borderColor: '#1e293b' }]
  };
  quantitiesData: ChartData<'bar'> = {
    labels: [],
    datasets: [{ data: [], label: 'Quantity Available', backgroundColor: '#3b82f6' }]
  };
  reordersData: ChartData<'bar'> = {
    labels: [],
    datasets: [{ data: [], label: 'Shortage (Units Needed)', backgroundColor: '#f43f5e' }]
  };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any[]>(`${this.apiUrl}/inventory/parts`).subscribe({
      next: (parts) => {
        let low = 0, ok = 0;
        const lowItems: any[] = [];
        
        parts.forEach(p => {
          if (p.quantityAvailable <= p.reorderLevel) {
            low++;
            lowItems.push(p);
          } else {
            ok++;
          }
        });
        
        // 1. Stock Health Pie
        this.stockHealthData = {
          ...this.stockHealthData,
          datasets: [{ ...this.stockHealthData.datasets[0], data: [low, ok] }]
        };
        
        // 2. Quantities (Top 8 by volume)
        const sortedByQty = [...parts].sort((a, b) => b.quantityAvailable - a.quantityAvailable).slice(0, 8);
        this.quantitiesData = { 
          labels: sortedByQty.map(p => p.name), 
          datasets: [{ ...this.quantitiesData.datasets[0], data: sortedByQty.map(p => p.quantityAvailable) }] 
        };

        // 3. Reorders (Shortage amount)
        // Shortage = ReorderLevel - Quantity (roughly how much we are "under")
        this.reordersData = { 
          labels: lowItems.map(p => p.name), 
          datasets: [{ 
            ...this.reordersData.datasets[0], 
            data: lowItems.map(p => (p.reorderLevel - p.quantityAvailable) + 5) // +5 buffer
          }] 
        };
      },
      error: (err) => console.error('Failed to load inventory', err)
    });
  }
}