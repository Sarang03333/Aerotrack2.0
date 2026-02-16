import { Component, OnInit } from "@angular/core";
import { ReactiveFormsModule, FormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";
import { InventoryService } from "../../core/services/inventory.service";

@Component({
  selector: "app-part-form",
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: "./part-form.component.html",
})
export class PartFormComponent implements OnInit {
  mode: "new" | "edit" = "new";
  id: string | null = null;
  errorMessage: string | null = null;

  form = this.fb.group({
  // Updated Pattern: Matches 'SP-' followed by exactly 3 digits
  partId: ["", [Validators.required, Validators.pattern(/^SP-\d{3}$/)]], 
  name: ["", Validators.required],
  quantityAvailable: [0, [Validators.required, Validators.min(0)]],
  reorderLevel: [0, [Validators.required, Validators.min(0)]],
});

  constructor(
    private fb: FormBuilder,
    private inventoryService: InventoryService,
    private route: ActivatedRoute,
    private router: Router,
  ) {}

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get("id");
    if (this.id) {
      this.mode = "edit";
      this.inventoryService.getPart(this.id).subscribe({
        next: (p: any) => {
          if (p) {
            this.form.patchValue(p);
            this.form.get("partId")?.disable();
          }
        },
        error: () => (this.errorMessage = "Failed to load part details.")
      });
    }
  }

  save() {
  if (this.form.invalid) return;
  this.errorMessage = null;

  const v = this.form.getRawValue();
  // Using the live inventoryService instead of mock-data
  const request$ = this.mode === "new" 
    ? this.inventoryService.createPart(v) 
    : this.inventoryService.updatePart(this.id!, v);

  request$.subscribe({
    next: () => this.router.navigate(["/inventory"]),
    error: (err: any) => {
      if (err.status === 400 && err.error?.errors) {
        // Backend will send the error message defined in your DTO
        this.errorMessage = err.error.errors.PartId ? err.error.errors.PartId[0] : "Invalid Part Data";
      } else {
        this.errorMessage = "Server error: Check your connection to SQL Express.";
      }
    }
  });
}
}