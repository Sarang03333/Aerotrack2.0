import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgFor } from '@angular/common';
import { MockDataService } from '../../core/services/mock-data.service';

@Component({
  selector: 'app-maintenance-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgFor],
  templateUrl: './maintenance-form.component.html'
})
export class MaintenanceFormComponent implements OnInit {
  mode: 'new' | 'edit' = 'new';
  id: string | null = null;

  form = this.fb.group({
    taskId: ['', Validators.required],
    aircraftId: ['', Validators.required],
    scheduledDate: ['', Validators.required],
    status: ['PENDING', Validators.required],
    description: ['', Validators.required],
    isEmergency: [false],
    priority: ['Normal'] // Emergency | High | Normal | Low
  });

  aircraftIds: string[] = [];

  constructor(
    private fb: FormBuilder,
    private data: MockDataService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.data.aircraftList$.subscribe(list => this.aircraftIds = list.map(a => a.aircraftId));
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.mode = 'edit';
      const t = this.data.getTask(this.id);
      if (t) {
        this.form.patchValue(t as any);
        this.form.get('taskId')?.disable();
      }
    }

    // If emergency is toggled, lock priority to Emergency
    this.form.get('isEmergency')?.valueChanges.subscribe(isE => {
      if (isE) {
        this.form.get('priority')?.setValue('Emergency');
      } else {
        const current = this.form.get('priority')?.value;
        if (current === 'Emergency') this.form.get('priority')?.setValue('Normal');
      }
    });
  }

  save() {
    if (this.form.invalid) return;
    const v = this.form.getRawValue() as any;
    if (this.mode === 'new') this.data.addTask(v);
    else this.data.updateTask(this.id!, v);
    this.router.navigate(['/maintenance']);
  }
}