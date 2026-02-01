import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MockDataService } from '../../core/services/mock-data.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-aircraft-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './aircraft-form.component.html'
})
export class AircraftFormComponent implements OnInit {
  mode: 'new' | 'edit' = 'new';
  id: string | null = null;

  form = this.fb.group({
    aircraftId: ['', Validators.required],
    model: ['', Validators.required],
    category: ['Commercial', Validators.required]
    // Removed: complianceStatus (server-owned)
  });

  computedStatus: string | null = null;

  constructor(
    private fb: FormBuilder,
    public data: MockDataService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');

    if (this.id) {
      this.mode = 'edit';
      const a = this.data.getAircraft(this.id);
      if (a) {
        this.form.patchValue({
          aircraftId: a.aircraftId,
          model: a.model,
          category: a.category
        } as any);
        this.form.get('aircraftId')?.disable();
        this.computedStatus = a.complianceStatus || null;
      }
    } else {
      this.mode = 'new';
      this.computedStatus = 'Pending'; // default shown; set by server on create
    }
  }

  save() {
    if (this.form.invalid) return;
    const v = this.form.getRawValue() as any;
    if (this.mode === 'new') this.data.addAircraft(v);
    else this.data.updateAircraft(this.id!, v);
    this.router.navigate(['/aircraft']);
  }
}