import { Component } from "@angular/core";
import { AsyncPipe, NgFor, NgClass, DatePipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { MockDataService } from "../../core/services/mock-data.service";
import { SearchPipe } from "../../shared/pipes/search.pipe";
@Component({
  selector: "app-list-aircraft",
  standalone: true,
  imports: [
    AsyncPipe,
    NgFor,
    NgClass,
    DatePipe,
    FormsModule,
    RouterLink,
    SearchPipe,
  ],
  templateUrl: "./list-aircraft.component.html",
})
export class ListAircraftComponent {
  term = "";
  aircraft$ = this.data.aircraftList$;
  constructor(public data: MockDataService) {}
  remove(id: string) {
    this.data.deleteAircraft(id);
  }
}
