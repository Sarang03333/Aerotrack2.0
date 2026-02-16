import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgFor, NgIf } from '@angular/common';
import { ComplianceService } from '../../core/services/Compliance.service';
import { AircraftService } from '../../core/services/Aircraft.service';

@Component({
  selector: 'app-audit-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgFor],
  templateUrl: './audit-form.component.html'
})
export class AuditFormComponent implements OnInit {
  mode: 'new' | 'edit' = 'new';
  id: string | null = null;
  errorMessage: string | null = null;

  form = this.fb.group({
  // Matches the AUD-XXX pattern
  auditId: ['', [Validators.required, Validators.pattern(/^AUD-\d{3}$/)]],
  aircraftId: ['', Validators.required],
  date: ['', Validators.required],
  findings: ['No discrepancies.', [Validators.required, Validators.minLength(5)]],
  severity: ['None', Validators.required]
});

  aircraftIds: string[] = [];

  constructor(
    private fb: FormBuilder,
    private complianceService: ComplianceService,
    private aircraftService: AircraftService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    // Fetch live aircraft list for the dropdown
    this.aircraftService.getAircrafts().subscribe({
      next: (list: any[]) => this.aircraftIds = list.map(a => a.aircraftId),
      error: () => this.errorMessage = "Unable to load aircraft list."
    });

    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.mode = 'edit';
      this.complianceService.getAudit(this.id).subscribe({
        next: (a: any) => {
          if (a) {
            this.form.patchValue(a);
            this.form.get('auditId')?.disable();
          }
        }
      });
    }
  }

  save() {
    if (this.form.invalid) return;
    this.errorMessage = null;

    const v = this.form.getRawValue();
    const request$ = this.mode === 'new' 
      ? this.complianceService.createAudit(v) 
      : this.complianceService.updateAudit(this.id!, v);

    request$.subscribe({
      next: () => this.router.navigate(['/compliance']),
      error: (err: any) => {
        // Handle 400 validation or 500 object cycle errors
        if (err.status === 400 && err.error?.errors) {
          this.errorMessage = err.error.errors.AuditId ? err.error.errors.AuditId[0] : "Check audit details.";
        } else if (err.status === 500) {
          this.errorMessage = "Server error: Ensure reference cycle handling is enabled in Program.cs.";
        } else {
          this.errorMessage = "An unexpected error occurred.";
        }
      }
    });
  }
}