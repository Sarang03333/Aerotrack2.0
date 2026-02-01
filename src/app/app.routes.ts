import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ListAircraftComponent } from './features/aircraft/list-aircraft.component';
import { AircraftDashboardComponent } from './features/aircraft/aircraft-dashboard.component';
import { AircraftFormComponent } from './features/aircraft/aircraft-form.component';
import { ListMaintenanceComponent } from './features/maintenance/list-maintenance.component';
import { MaintenanceDashboardComponent } from './features/maintenance/maintenance-dashboard.component';
import { MaintenanceFormComponent } from './features/maintenance/maintenance-form.component';
import { ListPartsComponent } from './features/inventory/list-parts.component';
import { InventoryDashboardComponent } from './features/inventory/inventory-dashboard.component';
import { PartFormComponent } from './features/inventory/part-form.component';
import { AuditsComponent } from './features/compliance/audits.component';
import { ComplianceDashboardComponent } from './features/compliance/compliance-dashboard.component';
import { AuditFormComponent } from './features/compliance/audit-form.component';
import { ReportsComponent } from './addons/reports/reports.component';

export const routes: Routes = [
  { path:'', pathMatch:'full', redirectTo:'dashboard' },
  { path:'dashboard', component: DashboardComponent },

  { path:'aircraft', component: ListAircraftComponent },
  { path:'aircraft/dashboard', component: AircraftDashboardComponent },
  { path:'aircraft/new', component: AircraftFormComponent },
  { path:'aircraft/edit/:id', component: AircraftFormComponent },

  { path:'maintenance', component: ListMaintenanceComponent },
  { path:'maintenance/dashboard', component: MaintenanceDashboardComponent },
  { path:'maintenance/new', component: MaintenanceFormComponent },
  { path:'maintenance/edit/:id', component: MaintenanceFormComponent },

  { path:'inventory', component: ListPartsComponent },
  { path:'inventory/dashboard', component: InventoryDashboardComponent },
  { path:'inventory/new', component: PartFormComponent },
  { path:'inventory/edit/:id', component: PartFormComponent },

  { path:'compliance', component: AuditsComponent },
  { path:'compliance/dashboard', component: ComplianceDashboardComponent },
  { path:'compliance/new', component: AuditFormComponent },
  { path:'compliance/edit/:id', component: AuditFormComponent },

  { path:'reports', component: ReportsComponent },
  { path:'**', redirectTo:'dashboard' }
];
