export interface Aircraft {
  aircraftId: string;
  model: string;
  category: "Commercial" | "Defense" | "Cargo";
  complianceStatus: "Compliant" | "Pending" | "Non-Compliant";
  lastServiceDate?: string;
}
