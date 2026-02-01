import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData } from 'chart.js';
import { MockDataService } from '../../core/services/mock-data.service';

@Component({
  selector: 'app-compliance-dashboard',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './compliance-dashboard.component.html'
})
export class ComplianceDashboardComponent implements OnInit {
  complianceData: ChartData<'doughnut'> = {
    labels: ['Compliant', 'Pending', 'Non-Compliant'],
    datasets: [{ data: [0, 0, 0], backgroundColor: ['#22c55e','#eab308','#ef4444'] }]
  };
  auditsByMonthData: ChartData<'bar'> = {
    labels: ['01','02','03','04','05','06','07','08','09','10','11','12'],
    datasets: [{ data: new Array(12).fill(0), backgroundColor:'rgba(99,102,241,.4)', borderColor:'#6366f1' }]
  };

  constructor(public data: MockDataService) {}

  ngOnInit() {
    this.data.aircraftList$.subscribe(list => {
      const comp = [0,0,0];
      list.forEach(a => {
        if (a.complianceStatus === 'Compliant') comp[0]++;
        else if (a.complianceStatus === 'Pending') comp[1]++;
        else comp[2]++;
      });
      this.complianceData = { ...this.complianceData, datasets: [{ ...this.complianceData.datasets[0], data: comp }] };
    });

    this.data.auditList$.subscribe(list => {
      const m = new Array(12).fill(0);
      list.forEach(a => {
        const mm = new Date(a.date).getMonth();
        if (!isNaN(mm)) m[mm]++;
      });
      this.auditsByMonthData = { ...this.auditsByMonthData, datasets: [{ ...this.auditsByMonthData.datasets[0], data: m }] };
    });
  }
}