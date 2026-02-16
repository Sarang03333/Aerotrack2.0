import { Component } from "@angular/core";
import { AsyncPipe, NgFor, NgClass, DatePipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { MockDataService } from "../../core/services/mock-data.service";
import { SearchPipe } from "../../shared/pipes/search.pipe";
@Component({
  selector: "app-list-parts",
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
  templateUrl: "./list-parts.component.html",
})
export class ListPartsComponent {
  term = "";
  parts$ = this.data.partList$;
  constructor(public data: MockDataService) {}
  replenish(id: string) {
    this.data.replenishPart(id);
  }
  remove(id: string) {
    this.data.deletePart(id);
  }
}