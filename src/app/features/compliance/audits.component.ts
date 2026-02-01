import { Component } from "@angular/core";
import { AsyncPipe, NgFor, DatePipe } from "@angular/common";
import { RouterLink } from "@angular/router";
import { MockDataService } from "../../core/services/mock-data.service";
@Component({
  selector: "app-audits",
  standalone: true,
  imports: [AsyncPipe, NgFor, DatePipe, RouterLink],
  templateUrl: "./audits.component.html",
})
export class AuditsComponent {
  audits$ = this.data.auditList$;
  constructor(public data: MockDataService) {}
  remove(id: string) {
    this.data.deleteAudit(id);
  }
}
