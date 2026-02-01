import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { BaseChartDirective } from "ng2-charts";
import { ChartData } from "chart.js";
import { MockDataService } from "../../core/services/mock-data.service";
@Component({
  selector: "app-maintenance-dashboard",
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: "./maintenance-dashboard.component.html",
})
export class MaintenanceDashboardComponent implements OnInit {
  statusData: ChartData<"doughnut"> = {
    labels: ["PENDING", "IN_PROGRESS", "COMPLETED"],
    datasets: [
      { data: [0, 0, 0], backgroundColor: ["#64748b", "#60a5fa", "#22c55e"] },
    ],
  };
  tasksPerDayData: ChartData<"line"> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: "Tasks",
        backgroundColor: "rgba(96,165,250,.3)",
        borderColor: "#60a5fa",
        fill: true,
      },
    ],
  };
  topAircraftData: ChartData<"bar"> = {
    labels: [],
    datasets: [
      {
        data: [],
        backgroundColor: "rgba(234,179,8,.4)",
        borderColor: "#eab308",
      },
    ],
  };
  constructor(public data: MockDataService) {}
  ngOnInit() {
    this.data.taskList$.subscribe((list) => {
      const status = [0, 0, 0];
      const perDay = new Map<string, number>();
      const byAc = new Map<string, number>();
      list.forEach((t) => {
        if (t.status === "PENDING") status[0]++;
        else if (t.status === "IN_PROGRESS") status[1]++;
        else status[2]++;
        perDay.set(t.scheduledDate, (perDay.get(t.scheduledDate) || 0) + 1);
        byAc.set(t.aircraftId, (byAc.get(t.aircraftId) || 0) + 1);
      });
      this.statusData = {
        ...this.statusData,
        datasets: [{ ...this.statusData.datasets[0], data: status }],
      };
      const dLabels = Array.from(perDay.keys()).sort();
      const dData = dLabels.map((k) => perDay.get(k) || 0);
      this.tasksPerDayData = {
        labels: dLabels,
        datasets: [{ ...this.tasksPerDayData.datasets[0], data: dData }],
      };
      const acLabels = Array.from(byAc.keys());
      const acData = acLabels.map((k) => byAc.get(k) || 0);
      this.topAircraftData = {
        labels: acLabels,
        datasets: [{ ...this.topAircraftData.datasets[0], data: acData }],
      };
    });
  }
}
