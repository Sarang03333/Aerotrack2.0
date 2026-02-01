import { Component } from '@angular/core';
import { AsyncPipe, NgFor, NgClass, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MockDataService } from '../../core/services/mock-data.service';
import { SearchPipe } from '../../shared/pipes/search.pipe';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { MaintenanceTask } from '../../core/models/maintenance-task';

@Component({
  selector: 'app-list-maintenance',
  standalone: true,
  imports: [AsyncPipe, NgFor, NgClass, DatePipe, FormsModule, RouterLink, SearchPipe],
  templateUrl: './list-maintenance.component.html'
})
export class ListMaintenanceComponent {
  term = '';
  tasks$ = this.data.taskList$;

  // NEW: derive a simple boolean observable for the banner
  hasEmergency$: Observable<boolean> = this.tasks$.pipe(
    map((list: MaintenanceTask[] = []) => list.some(t => t.isEmergency))
  );

  constructor(public data: MockDataService) {}

  complete(id: string) { this.data.completeTask(id); }
  remove(id: string) { this.data.deleteTask(id); }
}