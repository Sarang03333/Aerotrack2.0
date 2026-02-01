import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, combineLatest, map } from "rxjs";
import { environment } from "../../../environments/environment";
import { Aircraft } from "../models/aircraft";
import { MaintenanceTask } from "../models/maintenance-task";
import { SparePart } from "../models/spare-part";
import { AuditLog } from "../models/audit-log";

export interface FleetMetrics {
  totalAircraft: number;
  openTasks: number;
  completedTasks: number;
  nonCompliant: number;
}

@Injectable({ providedIn: "root" })
export class MockDataService {
  private http = inject(HttpClient);
  private base = environment.apiBaseUrl;

  private _aircraft$ = new BehaviorSubject<Aircraft[]>([]);
  private _tasks$ = new BehaviorSubject<MaintenanceTask[]>([]);
  private _parts$ = new BehaviorSubject<SparePart[]>([]);
  private _audits$ = new BehaviorSubject<AuditLog[]>([]);

  aircraftList$ = this._aircraft$.asObservable();
  taskList$ = this._tasks$.asObservable();
  partList$ = this._parts$.asObservable();
  auditList$ = this._audits$.asObservable();

  metrics$ = combineLatest([this.aircraftList$, this.taskList$]).pipe(
    map(
      ([a, t]): FleetMetrics => ({
        totalAircraft: a.length,
        openTasks: t.filter((x) => x.status !== "COMPLETED").length,
        completedTasks: t.filter((x) => x.status === "COMPLETED").length,
        nonCompliant: a.filter((x) => x.complianceStatus !== "Compliant")
          .length,
      }),
    ),
  );

  constructor() {
    this.refreshAll();
  }

  // Refresh helpers
  refreshAircraft() {
    this.http
      .get<Aircraft[]>(`${this.base}/api/aircraft`)
      .subscribe((v) => this._aircraft$.next(v));
  }
  refreshTasks() {
    this.http
      .get<MaintenanceTask[]>(`${this.base}/api/maintenance/tasks`)
      .subscribe((v) => this._tasks$.next(v));
  }
  refreshParts() {
    this.http
      .get<SparePart[]>(`${this.base}/api/inventory/parts`)
      .subscribe((v) => this._parts$.next(v));
  }
  refreshAudits() {
    this.http
      .get<AuditLog[]>(`${this.base}/api/compliance/audits`)
      .subscribe((v) => this._audits$.next(v));
  }
  refreshAll() {
    this.refreshAircraft();
    this.refreshTasks();
    this.refreshParts();
    this.refreshAudits();
  }

  // Aircraft
  getAircraft(id: string) {
    return this._aircraft$.value.find((a) => a.aircraftId === id);
  }
  addAircraft(a: Aircraft) {
    return this.http
      .post(`${this.base}/api/aircraft`, a)
      .subscribe(() => this.refreshAircraft());
  }
  updateAircraft(id: string, patch: Partial<Aircraft>) {
    return this.http
      .put(`${this.base}/api/aircraft/${id}`, patch)
      .subscribe(() => this.refreshAircraft());
  }
  deleteAircraft(id: string) {
    return this.http.delete(`${this.base}/api/aircraft/${id}`).subscribe(() => {
      this.refreshAircraft();
      this.refreshTasks();
      this.refreshAudits();
    });
  }

  // Maintenance
  getTask(id: string) {
    return this._tasks$.value.find((t) => t.taskId === id);
  }
  
addTask(t: MaintenanceTask) {
  // ensure defaults if not provided
  t.isEmergency = !!t.isEmergency;
  t.priority = t.isEmergency ? 'Emergency' : (t.priority ?? 'Normal');
  return this.http.post(`${this.base}/api/maintenance/tasks`, t)
    .subscribe(() => this.refreshTasks());
}

 

updateTask(id: string, patch: Partial<MaintenanceTask>) {
  return this.http.put(`${this.base}/api/maintenance/tasks/${id}`, patch)
    .subscribe(() => { this.refreshTasks(); this.refreshAircraft(); });
}

createEmergency(aircraftId: string, description: string) {
  return this.http.post(`${this.base}/api/maintenance/tasks/emergency`, { aircraftId, description })
    .subscribe(() => { this.refreshTasks(); this.refreshAircraft(); });
}


  completeTask(id: string) {
    return this.http
      .post(`${this.base}/api/maintenance/tasks/${id}/complete`, {})
      .subscribe(() => {
        this.refreshTasks();
        this.refreshAircraft();
      });
  }
  deleteTask(id: string) {
    return this.http
      .delete(`${this.base}/api/maintenance/tasks/${id}`)
      .subscribe(() => this.refreshTasks());
  }

  // Inventory
  getPart(id: string) {
    return this._parts$.value.find((p) => p.partId === id);
  }
  addPart(p: SparePart) {
    return this.http
      .post(`${this.base}/api/inventory/parts`, p)
      .subscribe(() => this.refreshParts());
  }
  updatePart(id: string, patch: Partial<SparePart>) {
    return this.http
      .put(`${this.base}/api/inventory/parts/${id}`, patch)
      .subscribe(() => this.refreshParts());
  }
  deletePart(id: string) {
    return this.http
      .delete(`${this.base}/api/inventory/parts/${id}`)
      .subscribe(() => this.refreshParts());
  }
  replenishPart(id: string) {
    return this.http
      .post(`${this.base}/api/inventory/parts/${id}/replenish`, {})
      .subscribe(() => this.refreshParts());
  }

  // Compliance
  getAudit(id: string) {
    return this._audits$.value.find((a) => a.auditId === id);
  }

// Audits
addAudit(a: AuditLog) {
  return this.http.post(`${this.base}/api/compliance/audits`, a).subscribe(() => {
    this.refreshAudits();
    // Aircraft compliance may change
    this.refreshAircraft();
  });
}
updateAudit(id: string, patch: Partial<AuditLog>) {
  return this.http.put(`${this.base}/api/compliance/audits/${id}`, patch).subscribe(() => {
    this.refreshAudits();
    this.refreshAircraft();
  });
}
deleteAudit(id: string) {
  return this.http.delete(`${this.base}/api/compliance/audits/${id}`).subscribe(() => {
    this.refreshAudits();
    this.refreshAircraft();
  });
}
}
