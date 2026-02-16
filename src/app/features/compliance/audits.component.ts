import { Component, OnInit } from "@angular/core";
import { AsyncPipe, NgFor, DatePipe } from "@angular/common";
import { RouterLink } from "@angular/router";
import { ComplianceService } from "../../core/services/Compliance.service"; // Use live service
import { Observable } from 'rxjs';

@Component({
  selector: "app-audits",
  standalone: true,
  imports: [AsyncPipe, NgFor, DatePipe, RouterLink],
  templateUrl: "./audits.component.html",
})
export class AuditsComponent implements OnInit {
  audits$!: Observable<any[]>;

  constructor(private complianceService: ComplianceService) {}

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.audits$ = this.complianceService.getAudits();
  }

  remove(id: string) {
    if (confirm('Are you sure you want to delete this audit report?')) {
      // Calls the newly added service method
      this.complianceService.deleteAudit(id).subscribe({
        next: () => {
          this.refresh(); // Reload table from SQL Express
        },
        error: (err) => {
          console.error("Delete failed", err);
          alert("Server error: Could not delete the audit.");
        }
      });
    }
  }
}