import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData } from 'chart.js';
import { MockDataService } from '../../core/services/mock-data.service';

@Component({
  selector: 'app-inventory-dashboard',
  standalone: true,
  imports: [ BaseChartDirective],
  templateUrl: './inventory-dashboard.component.html'
})
export class InventoryDashboardComponent implements OnInit {
  stockHealthData: ChartData<'doughnut'> = {
    labels: ['Low', 'OK'],
    datasets: [{ data: [0, 0], backgroundColor: ['#ef4444', '#22c55e'] }]
  };
  quantitiesData: ChartData<'bar'> = {
    labels: [],
    datasets: [{ data: [], backgroundColor: 'rgba(34,197,94,.3)', borderColor: '#22c55e' }]
  };
  reordersData: ChartData<'bar'> = {
    labels: [],
    datasets: [{ data: [], backgroundColor: 'rgba(248,113,113,.4)', borderColor: '#ef4444' }]
  };

  constructor(public data: MockDataService) {}

  ngOnInit() {
    this.data.partList$.subscribe(parts => {
      let low = 0, ok = 0;
      parts.forEach(p => (p.quantityAvailable <= p.reorderLevel) ? low++ : ok++);

      this.stockHealthData = {
        ...this.stockHealthData,
        datasets: [{ ...this.stockHealthData.datasets[0], data: [low, ok] }]
      };

      const labels = parts.map(p => p.name);
      const qty = parts.map(p => p.quantityAvailable);
      this.quantitiesData = { labels, datasets: [{ ...this.quantitiesData.datasets[0], data: qty }] };

      const rLabels = parts.filter(p => p.quantityAvailable <= p.reorderLevel).map(p => p.name);
      const rData = parts.filter(p => p.quantityAvailable <= p.reorderLevel)
                         .map(p => p.reorderLevel - p.quantityAvailable);
      this.reordersData = { labels: rLabels, datasets: [{ ...this.reordersData.datasets[0], data: rData }] };
    });
  }
}