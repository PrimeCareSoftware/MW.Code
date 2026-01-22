import { Routes } from '@angular/router';
import { SoapRecordComponent } from './soap-record.component';
import { SoapListComponent } from './components/soap-list/soap-list.component';

export const SOAP_ROUTES: Routes = [
  {
    path: '',
    component: SoapListComponent
  },
  {
    path: 'new/:attendanceId',
    component: SoapRecordComponent
  },
  {
    path: ':id',
    component: SoapRecordComponent
  },
  {
    path: ':id/edit',
    component: SoapRecordComponent
  }
];
