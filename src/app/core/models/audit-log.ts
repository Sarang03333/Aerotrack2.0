export type AuditSeverity = 'None' | 'Minor' | 'Major' | 'Critical';

export interface AuditLog {
  auditId: string;
  aircraftId: string;
  findings: string;
  date: string;
  severity: AuditSeverity;
}