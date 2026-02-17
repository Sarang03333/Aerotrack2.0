import { Component, OnInit } from "@angular/core";
import { AsyncPipe, NgFor, NgClass, DatePipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { InventoryService } from "../../core/services/inventory.service"; // Switched to live service
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
    // Fetches fresh data from https://localhost:5001/api/Inventory/parts
    this.parts$ = this.inventoryService.getParts();
  }

  replenish(id: string) {
    // Implementation for live replenishment
  }

  remove(id: string) {
    if (confirm('Are you sure you want to delete this part?')) {
      this.inventoryService.deletePart(id).subscribe({
        next: () => this.refresh(), // UI updates immediately after DB deletion
        error: (err) => console.error("Delete failed", err)
      });
    }
  }
}