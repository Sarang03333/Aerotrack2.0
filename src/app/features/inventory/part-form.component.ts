import { Component, OnInit } from "@angular/core";
import { ReactiveFormsModule, FormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { MockDataService } from "../../core/services/mock-data.service";
@Component({
  selector: "app-part-form",
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: "./part-form.component.html",
})
export class PartFormComponent implements OnInit {
  mode: "new" | "edit" = "new";
  id: string | null = null;
  form = this.fb.group({
    partId: ["", Validators.required],
    name: ["", Validators.required],
    quantityAvailable: [0, Validators.min(0)],
    reorderLevel: [0, Validators.min(0)],
  });
  constructor(
    private fb: FormBuilder,
    private data: MockDataService,
    private route: ActivatedRoute,
    private router: Router,
  ) {}
  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get("id");
    if (this.id) {
      this.mode = "edit";
      const p = this.data.getPart(this.id);
      if (p) {
        this.form.patchValue(p as any);
        this.form.get("partId")?.disable();
      }
    }
  }
  save() {
    if (this.form.invalid) return;
    const v = this.form.getRawValue() as any;
    if (this.mode === "new") this.data.addPart(v);
    else this.data.updatePart(this.id!, v);
    this.router.navigate(["/inventory"]);
  }
}
