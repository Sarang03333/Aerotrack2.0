import { Routes } from '@angular/router';
import { LoginComponent } from './core/auth/login.component';
import { roleGuard } from './core/auth/role.guard';

export const routes: Routes = [
  // 1. Default -> Login (Eagerly Loaded)
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  { path: 'login', component: LoginComponent },

  { 
    path: 'dashboard', 
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [roleGuard(['Maintenance', 'InventoryManager', 'ComplianceOfficer'])] 
  },

  { 
    path: 'aircraft', 
    loadComponent: () => import('./features/aircraft/list-aircraft.component').then(m => m.ListAircraftComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'aircraft/dashboard', 
    loadComponent: () => import('./features/aircraft/aircraft-dashboard.component').then(m => m.AircraftDashboardComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'aircraft/new', 
    loadComponent: () => import('./features/aircraft/aircraft-form.component').then(m => m.AircraftFormComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'aircraft/edit/:id', 
    loadComponent: () => import('./features/aircraft/aircraft-form.component').then(m => m.AircraftFormComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },

  { 
    path: 'maintenance', 
    loadComponent: () => import('./features/maintenance/list-maintenance.component').then(m => m.ListMaintenanceComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'maintenance/dashboard', 
    loadComponent: () => import('./features/maintenance/maintenance-dashboard.component').then(m => m.MaintenanceDashboardComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'maintenance/new', 
    loadComponent: () => import('./features/maintenance/maintenance-form.component').then(m => m.MaintenanceFormComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'maintenance/edit/:id', 
    loadComponent: () => import('./features/maintenance/maintenance-form.component').then(m => m.MaintenanceFormComponent),
    canActivate: [roleGuard(['Maintenance'])]
  },
  { 
    path: 'inventory', 
    loadComponent: () => import('./features/inventory/list-parts.component').then(m => m.ListPartsComponent),
    canActivate: [roleGuard(['InventoryManager'])]
  },
  { 
    path: 'inventory/dashboard', 
    loadComponent: () => import('./features/inventory/inventory-dashboard.component').then(m => m.InventoryDashboardComponent),
    canActivate: [roleGuard(['InventoryManager'])]
  },
  { 
    path: 'inventory/new', 
    loadComponent: () => import('./features/inventory/part-form.component').then(m => m.PartFormComponent),
    canActivate: [roleGuard(['InventoryManager'])]
  },
  { 
    path: 'inventory/edit/:id', 
    loadComponent: () => import('./features/inventory/part-form.component').then(m => m.PartFormComponent),
    canActivate: [roleGuard(['InventoryManager'])]
  },

  { 
    path: 'compliance', 
    loadComponent: () => import('./features/compliance/audits.component').then(m => m.AuditsComponent),
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },
  { 
    path: 'compliance/dashboard', 
    loadComponent: () => import('./features/compliance/compliance-dashboard.component').then(m => m.ComplianceDashboardComponent),
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },
  { 
    path: 'compliance/new', 
    loadComponent: () => import('./features/compliance/audit-form.component').then(m => m.AuditFormComponent),
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },
  { 
    path: 'compliance/edit/:id', 
    loadComponent: () => import('./features/compliance/audit-form.component').then(m => m.AuditFormComponent),
    canActivate: [roleGuard(['ComplianceOfficer'])]
  },

  { 
    path: 'reports', 
    loadComponent: () => import('./features/reports/reports.component').then(m => m.ReportsComponent),
    canActivate: [roleGuard(['Admin', 'Maintenance', 'ComplianceOfficer','InventoryManager'])]
  },

  // Fallback
  { path: '**', redirectTo: 'dashboard' }
];