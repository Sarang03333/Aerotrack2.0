import { Component, OnInit } from "@angular/core";
import { AsyncPipe, NgFor, NgClass, DatePipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { InventoryService } from "../../core/services/inventory.service"
import { SearchPipe } from "../../shared/pipes/search.pipe";
import { Observable } from 'rxjs';

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
export class ListPartsComponent implements OnInit {
  term = "";
  parts$!: Observable<any[]>;

  constructor(private inventoryService: InventoryService) {}

  ngOnInit() {
    this.refresh(); // Triggers live fetch on component load
  }

  refresh() {
    this.parts$ = this.inventoryService.getParts();
  }

  replenish(id: string) {
  // 1. Call the service to update the database
  this.inventoryService.replenishPart(id).subscribe({
    next: () => {
      // 2. Refresh the parts$ observable to show the updated quantity
      this.refresh(); 
      console.log(`Part ${id} replenished successfully.`);
    },
    error: (err) => {
      console.error("Replenishment failed:", err);
      alert("Could not replenish part. Check if the backend is running.");
    }
  });
}

  remove(id: string) {
    if (confirm('Are you sure you want to delete this part?')) {
      this.inventoryService.deletePart(id).subscribe({
        next: () => this.refresh(),
        error: (err) => console.error("Delete failed", err)
      });
    }
  }
}