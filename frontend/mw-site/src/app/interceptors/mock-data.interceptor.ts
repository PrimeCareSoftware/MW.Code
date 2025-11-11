import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { of, delay } from 'rxjs';
import { AVAILABLE_PLANS } from '../models/subscription-plan.model';

/**
 * HTTP Interceptor for returning mocked data for mw-site
 * This allows the site to run without a backend API for static deployment
 */
export const mockDataInterceptor: HttpInterceptorFn = (req, next) => {
  // Always use mock data for static site
  const mockDelay = Math.floor(Math.random() * 300) + 200;

  const url = req.url;
  const method = req.method;

  // Plans endpoint
  if (url.includes('/registration/plans') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: AVAILABLE_PLANS })).pipe(delay(mockDelay));
  }

  // Registration endpoint - always return success for static site
  if (url.includes('/registration') && method === 'POST') {
    const mockResponse = {
      success: true,
      message: 'Registro realizado com sucesso! Em breve você receberá um email de confirmação.',
      tenantId: 'demo-clinic',
      username: (req.body as any)?.username || 'demo'
    };
    return of(new HttpResponse({ status: 201, body: mockResponse })).pipe(delay(mockDelay));
  }

  // Contact endpoint - always return success for static site
  if (url.includes('/contact') && method === 'POST') {
    const mockResponse = {
      success: true,
      message: 'Mensagem enviada com sucesso! Entraremos em contato em breve.'
    };
    return of(new HttpResponse({ status: 200, body: mockResponse })).pipe(delay(mockDelay));
  }

  // Check CNPJ endpoint - always return available for static site
  if (url.includes('/check-cnpj') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: { exists: false } })).pipe(delay(mockDelay));
  }

  // Check username endpoint - always return available for static site
  if (url.includes('/check-username') && method === 'GET') {
    return of(new HttpResponse({ status: 200, body: { available: true } })).pipe(delay(mockDelay));
  }

  // For any other request in static mode, return empty success
  return of(new HttpResponse({ status: 200, body: {} })).pipe(delay(mockDelay));
};
