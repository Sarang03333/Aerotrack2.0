import { Routes } from '@angular/router';
import { LoginComponent } from './core/auth/login.component';
import { roleGuard } from './core/auth/role.guard'; // Ensure path is correct

// Feature Components
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
import { ReportsComponent } from './features/reports/reports.component';




export const routes: Routes = [
  // 1. Default -> Login
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  
  // 2. Public Login
  { path: 'login', component: LoginComponent },

  // 3. Protected Dashboard (Any authenticated role)
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [roleGuard(['Maintenance', 'InventoryManager', 'ComplianceOfficer'])] 
  },

  // --- AIRCRAFT (Admin or Maintenance) ---
  { 
    path: 'aircraft', 
    component: ListAircraftComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'aircraft/dashboard', 
    component: AircraftDashboardComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'aircraft/new', 
    component: AircraftFormComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'aircraft/edit/:id', 
    component: AircraftFormComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },

  // --- MAINTENANCE (Admin or Maintenance) ---
  { 
    path: 'maintenance', 
    component: ListMaintenanceComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'maintenance/dashboard', 
    component: MaintenanceDashboardComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'maintenance/new', 
    component: MaintenanceFormComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'maintenance/edit/:id', 
    component: MaintenanceFormComponent,
    canActivate: [roleGuard(['Maintenance'])]
  },

  // --- INVENTORY (Admin or InventoryManager) ---
  { 
    path: 'inventory', 
    component: ListPartsComponent,
    canActivate: [roleGuard(['InventoryManager'])]
  },
  { 
    path: 'inventory/dashboard', 
    component: InventoryDashboardComponent,
    canActivate: [roleGuard(['InventoryManager'])]
  },
  { 
    path: 'inventory/new', 
    component: PartFormComponent,
    canActivate: [roleGuard(['InventoryManager'])]
  },
  { 
    path: 'inventory/edit/:id', 
    component: PartFormComponent,
    canActivate: [roleGuard(['InventoryManager'])]
  },

  // --- COMPLIANCE (Admin or ComplianceOfficer) ---
  { 
    path: 'compliance', 
    component: AuditsComponent,
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },
  { 
    path: 'compliance/dashboard', 
    component: ComplianceDashboardComponent,
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },
  { 
    path: 'compliance/new', 
    component: AuditFormComponent,
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },
  { 
    path: 'compliance/edit/:id', 
    component: AuditFormComponent,
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },

  // --- REPORTS ---
  { 
        path: 'reports', 
        component:ReportsComponent,
        canActivate: [roleGuard(['Admin', 'Maintenance', 'ComplianceOfficer'])],
        data: { roles: ['Admin', 'Maintenance', 'ComplianceOfficer'] } // Adjust roles as needed
    },

  // Fallback
  { path: '**', redirectTo: 'dashboard' }
];