import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { tap, catchError, shareReplay } from 'rxjs/operators';

export interface TerminologyMap {
  appointment: string;
  professional: string;
  registration: string;
  client: string;
  mainDocument: string;
  exitDocument: string;
}

@Injectable({
  providedIn: 'root'
})
export class TerminologyService {
  private readonly baseUrl = '/api/BusinessConfiguration';
  private terminologyCache$ = new BehaviorSubject<TerminologyMap | null>(null);
  private loading = false;

  // Default terminology (Medical specialty)
  private readonly defaultTerminology: TerminologyMap = {
    appointment: 'Consulta',
    professional: 'Médico',
    registration: 'CRM',
    client: 'Paciente',
    mainDocument: 'Prontuário Médico',
    exitDocument: 'Receita Médica'
  };

  constructor(private http: HttpClient) {}

  /**
   * Load terminology for a specific clinic
   */
  loadTerminology(clinicId: string): Observable<TerminologyMap> {
    if (this.loading) {
      return this.terminologyCache$.asObservable() as Observable<TerminologyMap>;
    }

    this.loading = true;
    return this.http.get<TerminologyMap>(`${this.baseUrl}/clinic/${clinicId}/terminology`).pipe(
      tap(terminology => {
        this.terminologyCache$.next(terminology);
        this.loading = false;
      }),
      catchError(error => {
        console.error('Error loading terminology:', error);
        this.terminologyCache$.next(this.defaultTerminology);
        this.loading = false;
        return of(this.defaultTerminology);
      }),
      shareReplay(1)
    );
  }

  /**
   * Get the current terminology or default if not loaded
   */
  getTerminology(): TerminologyMap {
    return this.terminologyCache$.value || this.defaultTerminology;
  }

  /**
   * Get terminology as observable
   */
  getTerminology$(): Observable<TerminologyMap | null> {
    return this.terminologyCache$.asObservable();
  }

  /**
   * Get a specific term by key
   */
  getTerm(key: keyof TerminologyMap): string {
    const terminology = this.getTerminology();
    return terminology[key] || key;
  }

  /**
   * Clear the terminology cache
   */
  clearCache(): void {
    this.terminologyCache$.next(null);
    this.loading = false;
  }

  /**
   * Replace placeholders in a string with terminology
   */
  replacePlaceholders(text: string): string {
    const terminology = this.getTerminology();
    let result = text;
    
    Object.entries(terminology).forEach(([key, value]) => {
      const placeholder = `{{${key}}}`;
      result = result.replace(new RegExp(placeholder, 'g'), value);
    });
    
    return result;
  }
}
