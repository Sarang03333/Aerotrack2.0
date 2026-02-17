import { Component, OnInit } from "@angular/core";
import { AsyncPipe, NgFor, NgClass, DatePipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { AircraftService } from "../../core/services/Aircraft.service"; // Switched to live service
import { SearchPipe } from "../../shared/pipes/search.pipe";
import { Observable } from 'rxjs';

@Component({
  selector: "app-list-aircraft",
  standalone: true,
  imports: [AsyncPipe, NgFor, NgClass, DatePipe, FormsModule, RouterLink, SearchPipe],
  templateUrl: "./list-aircraft.component.html",
})
export class ListAircraftComponent implements OnInit {
  term = "";
  aircraft$!: Observable<any[]>;

  constructor(private aircraftService: AircraftService) {}

  ngOnInit() {
    this.refresh(); // Automatically fetches data on component load
  }

  refresh() {
    this.aircraft$ = this.aircraftService.getAircrafts();
  }

  remove(id: string) {
  if (confirm('Delete this aircraft?')) {
    this.aircraftService.deleteAircraft(id).subscribe({
      next: () => this.refresh(), // This ensures the list updates immediately
      error: (err) => console.error("Delete failed", err)
    });
  }
}
}