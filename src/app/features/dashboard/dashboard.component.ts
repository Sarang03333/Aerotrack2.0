import { Component, OnInit } from "@angular/core";
import { StatCardComponent } from "../../shared/components/stat-card/stat-card.component";
import { BaseChartDirective } from "ng2-charts";
import { ChartData } from "chart.js";
import { MockDataService } from "../../core/services/mock-data.service";
import { AsyncPipe } from "@angular/common";
@Component({
  selector: "app-dashboard",
  standalone: true,
  imports: [AsyncPipe, StatCardComponent, BaseChartDirective],
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.css"],
})
export class DashboardComponent implements OnInit {
  metrics$ = this.data.metrics$;
  maintenanceTrendData: ChartData<"line"> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: "Completed",
        borderColor: "#22c55e",
        backgroundColor: "rgba(34,197,94,.2)",
        fill: true,
      },
      {
        data: [],
        label: "Open",
        borderColor: "#60a5fa",
        backgroundColor: "rgba(96,165,250,.2)",
        fill: true,
      },
    ],
  };
  complianceData: ChartData<"doughnut"> = {
    labels: ["Compliant", "Pending", "Non-Compliant"],
    datasets: [
      { data: [0, 0, 0], backgroundColor: ["#22c55e", "#eab308", "#ef4444"] },
    ],
  };
  inventoryHealthData: ChartData<"bar"> = {
    labels: ["Low", "OK"],
    datasets: [{ data: [0, 0], backgroundColor: ["#ef4444", "#22c55e"] }],
  };
  constructor(public data: MockDataService) {}
  ngOnInit() {
    this.data.taskList$.subscribe((list) => {
      const labels = list.map((t) => t.scheduledDate).slice(0, 8);
      const completed = list
        .slice(0, 8)
        .map((t) => (t.status === "COMPLETED" ? 1 : 0));
      const open = list
        .slice(0, 8)
        .map((t) => (t.status !== "COMPLETED" ? 1 : 0));
      this.maintenanceTrendData = {
        labels,
        datasets: [
          { ...this.maintenanceTrendData.datasets[0], data: completed },
          { ...this.maintenanceTrendData.datasets[1], data: open },
        ],
      };
    });
    this.data.aircraftList$.subscribe((list) => {
      const comp = [0, 0, 0];
      list.forEach((a) => {
        if (a.complianceStatus === "Compliant") comp[0]++;
        else if (a.complianceStatus === "Pending") comp[1]++;
        else comp[2]++;
      });
      this.complianceData = {
        ...this.complianceData,
        datasets: [{ ...this.complianceData.datasets[0], data: comp }],
      };
    });
    this.data.partList$.subscribe((parts) => {
      let low = 0,
        ok = 0;
      parts.forEach((p) =>
        p.quantityAvailable <= p.reorderLevel ? low++ : ok++,
      );
      this.inventoryHealthData = {
        ...this.inventoryHealthData,
        datasets: [
          { ...this.inventoryHealthData.datasets[0], data: [low, ok] },
        ],
      };
    });
  }
}
