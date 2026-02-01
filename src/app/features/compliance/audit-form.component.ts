import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgFor } from '@angular/common';
import { MockDataService } from '../../core/services/mock-data.service';

@Component({
  selector: 'app-audit-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgFor],
  templateUrl: './audit-form.component.html'
})
export class AuditFormComponent implements OnInit {
  mode: 'new' | 'edit' = 'new';
  id: string | null = null;

  form = this.fb.group({
    auditId: ['', Validators.required],
    aircraftId: ['', Validators.required],
    date: ['', Validators.required],
    findings: ['No discrepancies.', Validators.required],
    severity: ['None', Validators.required] // NEW
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
      const a = this.data.getAudit(this.id);
      if (a) {
        this.form.patchValue(a as any);
        this.form.get('auditId')?.disable();
      }
    }
  }

  save() {
    if (this.form.invalid) return;
    const v = this.form.getRawValue() as any;
    if (this.mode === 'new') this.data.addAudit(v);
    else this.data.updateAudit(this.id!, v);
    this.router.navigate(['/compliance']);
  }
}