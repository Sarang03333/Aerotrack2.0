import { Component } from '@angular/core';
import { AsyncPipe, NgFor } from '@angular/common';
import { MockDataService } from '../../core/services/mock-data.service';
import { exportToCsv } from './csv.util';
@Component({ selector:'app-reports', standalone:true, imports:[AsyncPipe, NgFor], templateUrl:'./reports.component.html' })
export class ReportsComponent{
  aircraft$=this.data.aircraftList$; tasks$=this.data.taskList$; parts$=this.data.partList$; audits$=this.data.auditList$;
  constructor(public data:MockDataService){}
  exportAircraft(){ this.aircraft$.subscribe(list=> exportToCsv('aircraft.csv', list)); }
  exportTasks(){ this.tasks$.subscribe(list=> exportToCsv('maintenance.csv', list)); }
  exportParts(){ this.parts$.subscribe(list=> exportToCsv('spares.csv', list)); }
  exportAudits(){ this.audits$.subscribe(list=> exportToCsv('audits.csv', list)); }
}
