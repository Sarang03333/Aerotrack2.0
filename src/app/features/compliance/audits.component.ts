import { Component, OnInit } from "@angular/core";
import { AsyncPipe, NgFor, DatePipe } from "@angular/common";
import { RouterLink } from "@angular/router";
import { ComplianceService } from "../../core/services/Compliance.service"; 
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
    this.refresh(); // Reloads data every time user enters the compliance list
  }

  refresh() {
    this.audits$ = this.complianceService.getAudits();
  }

  remove(id: string) {
    if (confirm('Are you sure you want to delete this audit report?')) {
      this.complianceService.deleteAudit(id).subscribe({
        next: () => {
          this.refresh(); // UI reflects deletion immediately
        },
        error: (err) => {
          console.error("Delete failed", err);
          alert("Server error: Could not delete the audit.");
        }
      });
    }
  }
}