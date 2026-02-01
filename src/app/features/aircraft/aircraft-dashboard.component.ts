import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { BaseChartDirective } from "ng2-charts";
import { ChartData } from "chart.js";
import { MockDataService } from "../../core/services/mock-data.service";
@Component({
  selector: "app-aircraft-dashboard",
  standalone: true,
  imports: [ BaseChartDirective],
  templateUrl: "./aircraft-dashboard.component.html",
})
export class AircraftDashboardComponent implements OnInit {
  categoryData: ChartData<"doughnut"> = {
    labels: ["Commercial", "Defense", "Cargo"],
    datasets: [
      { data: [0, 0, 0], backgroundColor: ["#60a5fa", "#f59e0b", "#10b981"] },
    ],
  };
  complianceData: ChartData<"pie"> = {
    labels: ["Compliant", "Pending", "Non-Compliant"],
    datasets: [
      { data: [0, 0, 0], backgroundColor: ["#22c55e", "#eab308", "#ef4444"] },
    ],
  };
  servicesByMonthData: ChartData<"bar"> = {
    labels: [
      "01",
      "02",
      "03",
      "04",
      "05",
      "06",
      "07",
      "08",
      "09",
      "10",
      "11",
      "12",
    ],
    datasets: [
      {
        data: new Array(12).fill(0),
        label: "Service Events",
        backgroundColor: "rgba(99,102,241,.4)",
        borderColor: "#6366f1",
      },
    ],
  };
  constructor(public data: MockDataService) {}
  ngOnInit() {
    this.data.aircraftList$.subscribe((list) => {
      const cat = [0, 0, 0];
      const comp = [0, 0, 0];
      const months = new Array(12).fill(0);
      list.forEach((a) => {
        if (a.category === "Commercial") cat[0]++;
        else if (a.category === "Defense") cat[1]++;
        else cat[2]++;
        if (a.complianceStatus === "Compliant") comp[0]++;
        else if (a.complianceStatus === "Pending") comp[1]++;
        else comp[2]++;
        const d = a.lastServiceDate ? new Date(a.lastServiceDate) : null;
        if (d && !isNaN(d.getMonth())) months[d.getMonth()]++;
      });
      this.categoryData = {
        ...this.categoryData,
        datasets: [{ ...this.categoryData.datasets[0], data: cat }],
      };
      this.complianceData = {
        ...this.complianceData,
        datasets: [{ ...this.complianceData.datasets[0], data: comp }],
      };
      this.servicesByMonthData = {
        ...this.servicesByMonthData,
        datasets: [{ ...this.servicesByMonthData.datasets[0], data: months }],
      };
    });
  }
}
