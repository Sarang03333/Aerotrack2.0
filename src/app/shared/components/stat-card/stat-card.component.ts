import { Component, Input } from "@angular/core";
import { NgClass } from "@angular/common";
@Component({
  selector: "app-stat-card",
  standalone: true,
  imports: [NgClass],
  templateUrl: "./stat-card.component.html",
  styleUrls: ["./stat-card.component.css"],
})
export class StatCardComponent {
  @Input() label = "";
  @Input() value: any = "";
  @Input() subtitle = "";
  @Input() icon = "bi-speedometer2";
}
