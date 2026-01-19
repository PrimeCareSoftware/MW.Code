import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { of, delay } from 'rxjs';
import { environment } from '../../environments/environment';

// Import mock data
import { MOCK_PATIENTS } from '../mocks/patient.mock';
import { MOCK_APPOINTMENTS, MOCK_DAILY_AGENDA, MOCK_AVAILABLE_SLOTS } from '../mocks/appointment.mock';
import { MOCK_AUTH_RESPONSE, MOCK_USER_INFO } from '../mocks/auth.mock';
import { MOCK_PROCEDURES, MOCK_APPOINTMENT_PROCEDURES, MOCK_BILLING_SUMMARY } from '../mocks/procedure.mock';
import { MOCK_EXAM_REQUESTS, MOCK_PENDING_EXAMS, MOCK_URGENT_EXAMS } from '../mocks/exam-request.mock';
import { MOCK_MEDICAL_RECORDS } from '../mocks/medical-record.mock';
import { MOCK_QUEUE_ENTRIES, MOCK_QUEUE_SUMMARY, MOCK_QUEUE_CONFIGURATION, MOCK_PUBLIC_QUEUE_DISPLAY } from '../mocks/waiting-queue.mock';

/**
 * HTTP Interceptor for returning mocked data when useMockData flag is enabled
 * This allows the frontend to run without a backend API for testing and development
 */
export const mockDataInterceptor: HttpInterceptorFn = (req, next) => {
  // Only intercept if mock data is enabled
  if (!environment.useMockData) {
    return next(req);
  }

  // Simulate network delay (200-500ms)
  const mockDelay = Math.floor(Math.random() * 300) + 200;

  const url = req.url;
  const method = req.method;

  // Auth endpoints
  if (url.includes('/auth/login') && method === 'POST') {
    return of(new HttpResponse({ status: 200, body: MOCK_AUTH_RESPONSE })).pipe(delay(mockDelay));
  }
  if (url.includes('/auth/me') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_USER_INFO })).pipe(delay(mockDelay));
  }

  // Patient endpoints
  if (url.match(/\/patients$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_PATIENTS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/patients\/[^/]+$/) && method === 'GET') {
    const id = url.split('/').pop();
    const patient = MOCK_PATIENTS.find(p => p.id === id);
    return of(new HttpResponse({ status: 200, body: patient })).pipe(delay(mockDelay));
  }
  if (url.match(/\/patients$/) && method === 'POST') {
    const newPatient = { ...(req.body as any), id: `${MOCK_PATIENTS.length + 1}`, createdAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 201, body: newPatient })).pipe(delay(mockDelay));
  }
  if (url.match(/\/patients\/[^/]+$/) && method === 'PUT') {
    const id = url.split('/').pop();
    const patient = MOCK_PATIENTS.find(p => p.id === id);
    const updated = { ...patient, ...(req.body as any), updatedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 200, body: updated })).pipe(delay(mockDelay));
  }
  if (url.match(/\/patients\/[^/]+$/) && method === 'DELETE') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }
  if (url.includes('/patients/search') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_PATIENTS.slice(0, 2) })).pipe(delay(mockDelay));
  }
  if (url.match(/\/patients\/[^/]+\/children$/) && method === 'GET') {
    const children = MOCK_PATIENTS.filter(p => p.isChild);
    return of(new HttpResponse({ status: 200, body: children })).pipe(delay(mockDelay));
  }
  if (url.match(/\/patients\/[^/]+\/link-guardian\/[^/]+$/) && method === 'POST') {
    return of(new HttpResponse({ status: 200, body: { success: true } })).pipe(delay(mockDelay));
  }

  // Appointment endpoints
  if (url.includes('/appointments/agenda') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_DAILY_AGENDA })).pipe(delay(mockDelay));
  }
  if (url.includes('/appointments/available-slots') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_AVAILABLE_SLOTS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/appointments\/[^/]+$/) && method === 'GET') {
    const id = url.split('/').pop();
    const appointment = MOCK_APPOINTMENTS.find(a => a.id === id);
    return of(new HttpResponse({ status: 200, body: appointment })).pipe(delay(mockDelay));
  }
  if (url.match(/\/appointments$/) && method === 'POST') {
    const newAppointment = { ...(req.body as any), id: `${MOCK_APPOINTMENTS.length + 1}`, createdAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 201, body: newAppointment })).pipe(delay(mockDelay));
  }
  if (url.match(/\/appointments\/[^/]+$/) && method === 'PUT') {
    const id = url.split('/').pop();
    const appointment = MOCK_APPOINTMENTS.find(a => a.id === id);
    const updated = { ...appointment, ...(req.body as any), updatedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 200, body: updated })).pipe(delay(mockDelay));
  }
  if (url.match(/\/appointments\/[^/]+\/cancel$/) && method === 'PUT') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }

  // Procedure endpoints
  if (url.match(/\/procedures$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_PROCEDURES })).pipe(delay(mockDelay));
  }
  if (url.match(/\/procedures\/[^/]+$/) && method === 'GET') {
    const id = url.split('/').pop();
    const procedure = MOCK_PROCEDURES.find(p => p.id === id);
    return of(new HttpResponse({ status: 200, body: procedure })).pipe(delay(mockDelay));
  }
  if (url.match(/\/procedures$/) && method === 'POST') {
    const newProcedure = { ...(req.body as any), id: `${MOCK_PROCEDURES.length + 1}`, createdAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 201, body: newProcedure })).pipe(delay(mockDelay));
  }
  if (url.match(/\/procedures\/[^/]+$/) && method === 'PUT') {
    const id = url.split('/').pop();
    const procedure = MOCK_PROCEDURES.find(p => p.id === id);
    const updated = { ...procedure, ...(req.body as any), updatedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 200, body: updated })).pipe(delay(mockDelay));
  }
  if (url.match(/\/procedures\/[^/]+$/) && method === 'DELETE') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }
  if (url.match(/\/procedures\/appointments\/[^/]+\/procedures$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_APPOINTMENT_PROCEDURES })).pipe(delay(mockDelay));
  }
  if (url.match(/\/procedures\/appointments\/[^/]+\/procedures$/) && method === 'POST') {
    const newProc = { ...(req.body as any), id: `${MOCK_APPOINTMENT_PROCEDURES.length + 1}`, performedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 201, body: newProc })).pipe(delay(mockDelay));
  }
  if (url.match(/\/procedures\/appointments\/[^/]+\/billing-summary$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_BILLING_SUMMARY })).pipe(delay(mockDelay));
  }

  // Exam request endpoints
  if (url.includes('/exam-requests/pending') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_PENDING_EXAMS })).pipe(delay(mockDelay));
  }
  if (url.includes('/exam-requests/urgent') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_URGENT_EXAMS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/exam-requests\/appointment\/[^/]+$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_EXAM_REQUESTS.slice(0, 2) })).pipe(delay(mockDelay));
  }
  if (url.match(/\/exam-requests\/patient\/[^/]+$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_EXAM_REQUESTS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/exam-requests\/[^/]+$/) && method === 'GET') {
    const id = url.split('/').pop();
    const examRequest = MOCK_EXAM_REQUESTS.find(e => e.id === id);
    return of(new HttpResponse({ status: 200, body: examRequest })).pipe(delay(mockDelay));
  }
  if (url.match(/\/exam-requests$/) && method === 'POST') {
    const newExam = { ...(req.body as any), id: `${MOCK_EXAM_REQUESTS.length + 1}`, createdAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 201, body: newExam })).pipe(delay(mockDelay));
  }
  if (url.match(/\/exam-requests\/[^/]+$/) && method === 'PUT') {
    const id = url.split('/').pop();
    const examRequest = MOCK_EXAM_REQUESTS.find(e => e.id === id);
    const updated = { ...examRequest, ...(req.body as any), updatedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 200, body: updated })).pipe(delay(mockDelay));
  }
  if (url.match(/\/exam-requests\/[^/]+\/complete$/) && method === 'POST') {
    const id = url.split('/')[url.split('/').length - 2];
    const examRequest = MOCK_EXAM_REQUESTS.find(e => e.id === id);
    const completed = { ...examRequest, ...(req.body as any), completedDate: new Date().toISOString(), updatedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 200, body: completed })).pipe(delay(mockDelay));
  }
  if (url.match(/\/exam-requests\/[^/]+\/cancel$/) && method === 'POST') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }

  // Medical record endpoints
  if (url.match(/\/medical-records\/appointment\/[^/]+$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_MEDICAL_RECORDS[0] })).pipe(delay(mockDelay));
  }
  if (url.match(/\/medical-records\/patient\/[^/]+$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_MEDICAL_RECORDS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/medical-records$/) && method === 'POST') {
    const newRecord = { ...(req.body as any), id: `${MOCK_MEDICAL_RECORDS.length + 1}`, createdAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 201, body: newRecord })).pipe(delay(mockDelay));
  }
  if (url.match(/\/medical-records\/[^/]+$/) && method === 'PUT') {
    const id = url.split('/').pop();
    const record = MOCK_MEDICAL_RECORDS.find(r => r.id === id);
    const updated = { ...record, ...(req.body as any), updatedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 200, body: updated })).pipe(delay(mockDelay));
  }
  if (url.match(/\/medical-records\/[^/]+\/complete$/) && method === 'POST') {
    const id = url.split('/')[url.split('/').length - 2];
    const record = MOCK_MEDICAL_RECORDS.find(r => r.id === id);
    const completed = { ...record, ...(req.body as any), updatedAt: new Date().toISOString() };
    return of(new HttpResponse({ status: 200, body: completed })).pipe(delay(mockDelay));
  }

  // Waiting queue endpoints
  if (url.match(/\/waiting-queue\/clinic\/[^/]+$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_QUEUE_ENTRIES })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/clinic\/[^/]+\/summary$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_QUEUE_SUMMARY })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/clinic\/[^/]+\/configuration$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_QUEUE_CONFIGURATION })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/clinic\/[^/]+\/configuration$/) && method === 'PUT') {
    const updated = { ...MOCK_QUEUE_CONFIGURATION, ...(req.body as any) };
    return of(new HttpResponse({ status: 200, body: updated })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/clinic\/[^/]+\/public$/) && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_PUBLIC_QUEUE_DISPLAY })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue$/) && method === 'POST') {
    const newEntry = { ...(req.body as any), id: `${MOCK_QUEUE_ENTRIES.length + 1}`, createdAt: new Date() };
    return of(new HttpResponse({ status: 201, body: newEntry })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/[^/]+\/call$/) && method === 'POST') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/[^/]+\/complete$/) && method === 'POST') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/[^/]+\/cancel$/) && method === 'DELETE') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }
  if (url.match(/\/waiting-queue\/[^/]+\/triage$/) && method === 'PUT') {
    return of(new HttpResponse({ status: 204 })).pipe(delay(mockDelay));
  }

  // If no mock matches, pass through to real API
  console.warn(`Mock data interceptor: No mock handler for ${method} ${url}`);
  return next(req);
};
