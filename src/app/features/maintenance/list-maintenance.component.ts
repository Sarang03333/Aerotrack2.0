import { Component, OnInit } from '@angular/core';
import { AsyncPipe, NgFor, NgClass, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MaintenanceService } from '../../core/services/Maintenance.service'; 
import { SearchPipe } from '../../shared/pipes/search.pipe';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-list-maintenance',
  standalone: true,
  imports: [AsyncPipe, NgFor, NgClass, DatePipe, FormsModule, RouterLink, SearchPipe],
  templateUrl: './list-maintenance.component.html'
})
export class ListMaintenanceComponent implements OnInit {
  term = '';
  tasks$!: Observable<any[]>;
  hasEmergency$!: Observable<boolean>;

  constructor(private maintenanceService: MaintenanceService) {}

  ngOnInit() {
    this.refresh(); 
  }
  refresh() {
    this.tasks$ = this.maintenanceService.getTasks();
    this.hasEmergency$ = this.tasks$.pipe(
      map((list: any[] = []) => list.some(t => t.isEmergency))
    );
  }

  complete(id: string) {
    // Calling refresh inside subscribe ensures the UI updates after the DB change
    this.maintenanceService.completeTask(id).subscribe({
      next: () => this.refresh(), 
      error: (err: any) => console.error("Completion failed", err)
    });
  }

  remove(id: string) {
    if (confirm('Are you sure you want to delete this task?')) {
      this.maintenanceService.deleteTask(id).subscribe({
        next: () => this.refresh(),
        error: (err: any) => console.error("Delete failed", err)
      });
    }
  }
}