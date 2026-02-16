import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgFor, NgIf } from '@angular/common';

// FIX: Ensure these paths point correctly to your core services folder
import { MaintenanceService } from '../../core/services/Maintenance.service'; 
import { AircraftService } from '../../core/services/Aircraft.service';

@Component({
  selector: 'app-maintenance-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgFor, NgIf],
  templateUrl: './maintenance-form.component.html'
})
export class MaintenanceFormComponent implements OnInit {
  mode: 'new' | 'edit' = 'new';
  id: string | null = null;
  errorMessage: string | null = null;

  form = this.fb.group({
    taskId: ['', [Validators.required, Validators.pattern(/^MT-\d{4}-\d{3}$/)]],
    aircraftId: ['', Validators.required],
    scheduledDate: ['', Validators.required],
    status: ['PENDING', Validators.required],
    description: ['', [Validators.required, Validators.minLength(10)]],
    isEmergency: [false],
    priority: ['Normal']
  });

  aircraftIds: string[] = [];

  // FIX: Parameter names are private to avoid "Value not found" errors
  constructor(
    private fb: FormBuilder,
    private maintenanceService: MaintenanceService, 
    private aircraftService: AircraftService,       
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    // Load live aircraft IDs from your SQL Express backend
    this.aircraftService.getAircrafts().subscribe({
      next: (list) => this.aircraftIds = list.map(a => a.aircraftId),
      error: () => this.errorMessage = "Could not load aircraft list."
    });

    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.mode = 'edit';
      this.maintenanceService.getTask(this.id).subscribe(t => {
        if (t) {
          this.form.patchValue(t as any);
          this.form.get('taskId')?.disable();
        }
      });
    }
  }

  save() {
    if (this.form.invalid) return;
    this.errorMessage = null;

    const v = this.form.getRawValue();
    const request$ = this.mode === 'new' 
      ? this.maintenanceService.createTask(v) 
      : this.maintenanceService.updateTask(this.id!, v);

    request$.subscribe({
      next: () => this.router.navigate(['/maintenance']),
      error: (err) => {
        // Handle the 400 Bad Request from the AeroTrack API
        if (err.status === 400 && err.error?.errors) {
          const errors = err.error.errors;
          this.errorMessage = errors.TaskId ? errors.TaskId[0] : "Validation failed.";
        } else {
          this.errorMessage = "An error occurred while saving.";
        }
      }
    });
  }
}