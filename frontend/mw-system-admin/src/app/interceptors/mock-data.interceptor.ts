import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { of, delay } from 'rxjs';
import { environment } from '../../environments/environment';

// Import mock data
import { MOCK_AUTH_RESPONSE, MOCK_USER_INFO } from '../mocks/auth.mock';
import { 
  MOCK_CLINICS, 
  MOCK_CLINIC_DETAIL, 
  MOCK_PAGINATED_CLINICS, 
  MOCK_SYSTEM_ANALYTICS,
  MOCK_SYSTEM_OWNERS 
} from '../mocks/system-admin.mock';

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

  // System admin endpoints
  if (url.includes('/system-admin/clinics') && method === 'GET' && !url.match(/\/system-admin\/clinics\/[^/]+$/)) {
    return of(new HttpResponse({ status: 200, body: MOCK_PAGINATED_CLINICS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/clinics\/[^/]+$/) && method === 'GET') {
    const id = url.split('/').pop();
    const clinic = id === 'clinic1' ? MOCK_CLINIC_DETAIL : { ...MOCK_CLINICS.find(c => c.id === id) };
    return of(new HttpResponse({ status: 200, body: clinic })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/clinics$/) && method === 'POST') {
    const newClinic = { 
      ...(req.body as any), 
      id: `clinic${MOCK_CLINICS.length + 1}`,
      tenantId: `clinic${MOCK_CLINICS.length + 1}`,
      createdAt: new Date().toISOString() 
    };
    return of(new HttpResponse({ 
      status: 201, 
      body: { message: 'Clínica criada com sucesso', clinicId: newClinic.id } 
    })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/clinics\/[^/]+\/toggle-status$/) && method === 'POST') {
    const id = url.split('/')[url.split('/').length - 2];
    const clinic = MOCK_CLINICS.find(c => c.id === id);
    const newStatus = !clinic?.isActive;
    return of(new HttpResponse({ 
      status: 200, 
      body: { message: 'Status alterado com sucesso', isActive: newStatus } 
    })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/clinics\/[^/]+\/subscription$/) && method === 'PUT') {
    return of(new HttpResponse({ 
      status: 200, 
      body: { message: 'Assinatura atualizada com sucesso' } 
    })).pipe(delay(mockDelay));
  }
  if (url.includes('/system-admin/analytics') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: MOCK_SYSTEM_ANALYTICS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/clinics\/[^/]+\/subscription\/manual-override\/enable$/) && method === 'POST') {
    return of(new HttpResponse({ 
      status: 200, 
      body: { message: 'Override manual habilitado' } 
    })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/clinics\/[^/]+\/subscription\/manual-override\/disable$/) && method === 'POST') {
    return of(new HttpResponse({ 
      status: 200, 
      body: { message: 'Override manual desabilitado' } 
    })).pipe(delay(mockDelay));
  }

  // System owners endpoints
  if (url.includes('/system-admin/system-owners') && method === 'GET' && !url.match(/\/system-admin\/system-owners\/[^/]+$/)) {
    return of(new HttpResponse({ status: 200, body: MOCK_SYSTEM_OWNERS })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/system-owners$/) && method === 'POST') {
    const newOwner = { 
      ...(req.body as any), 
      id: `${MOCK_SYSTEM_OWNERS.length + 1}`,
      isActive: true,
      createdAt: new Date().toISOString() 
    };
    return of(new HttpResponse({ 
      status: 201, 
      body: { message: 'System Owner criado com sucesso', ownerId: newOwner.id } 
    })).pipe(delay(mockDelay));
  }
  if (url.match(/\/system-admin\/system-owners\/[^/]+\/toggle-status$/) && method === 'POST') {
    const id = url.split('/')[url.split('/').length - 2];
    const owner = MOCK_SYSTEM_OWNERS.find(o => o.id === id);
    const newStatus = !owner?.isActive;
    return of(new HttpResponse({ 
      status: 200, 
      body: { message: 'Status alterado com sucesso', isActive: newStatus } 
    })).pipe(delay(mockDelay));
  }

  // If no mock matches, pass through to real API
  console.warn(`Mock data interceptor: No mock handler for ${method} ${url}`);
  return next(req);
};
