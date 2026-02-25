import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { AircraftService } from '../../core/services/Aircraft.service';

@Component({
  selector: 'app-aircraft-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf],
  templateUrl: './aircraft-form.component.html'
})
export class AircraftFormComponent implements OnInit {
  mode: 'new' | 'edit' = 'new';
  id: string | null = null;
  errorMessage: string | null = null;

  form = this.fb.group({
    // Pattern matches the AC-XXX-000 convention
    aircraftId: ['', [Validators.required, Validators.pattern(/^AC-[A-Z]{3}-\d{3}$/)]],
    model: ['', Validators.required],
    category: ['Commercial', Validators.required]
  });

  computedStatus: string | null = null;

  constructor(
    private fb: FormBuilder,
    private aircraftService: AircraftService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');

    if (this.id) {
      this.mode = 'edit';
      this.aircraftService.getAircraft(this.id).subscribe({
        next: (a) => {
          if (a) {
            this.form.patchValue({
              aircraftId: a.aircraftId,
              model: a.model,
              category: a.category
            } as any);
            this.form.get('aircraftId')?.disable();
            this.computedStatus = a.complianceStatus || null;
          }
        },
        error: () => this.errorMessage = "Failed to load aircraft details."
      });
    } else {
      this.mode = 'new';
      this.computedStatus = 'Pending';
    }
  }

  save() {
    if (this.form.invalid) return;
    this.errorMessage = null;

    const v = this.form.getRawValue();
    const request$ = this.mode === 'new' 
      ? this.aircraftService.createAircraft(v) 
      : this.aircraftService.updateAircraft(this.id!, v);

    request$.subscribe({
      next: () => this.router.navigate(['/aircraft']),
      error: (err) => {
        // Parse the 400 Bad Request from the C# DTO validation
        if (err.status === 400 && err.error?.errors) {
          const errors = err.error.errors;
          this.errorMessage = errors.AircraftId ? errors.AircraftId[0] : "Check aircraft details.";
        } else {
          this.errorMessage = "Server error: Could not save aircraft.";
        }
      }
    });
  }
}