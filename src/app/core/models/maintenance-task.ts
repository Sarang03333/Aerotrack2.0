export type TaskPriority = 'Emergency' | 'High' | 'Normal' | 'Low';

export interface MaintenanceTask {
  taskId: string;
  aircraftId: string;
  scheduledDate: string; // ISO date (yyyy-MM-dd)
  status: 'PENDING' | 'IN_PROGRESS' | 'COMPLETED';
  description: string;

  // NEW
  isEmergency: boolean;
  priority: TaskPriority;
}