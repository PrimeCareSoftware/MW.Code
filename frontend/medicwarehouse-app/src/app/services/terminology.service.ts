import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { tap, catchError, shareReplay, filter } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface TerminologyMap {
  appointment: string;
  professional: string;
  registration: string;
  client: string;
  mainDocument: string;
  exitDocument: string;
}

export enum ProfessionalSpecialty {
  Medico = 'Medico',
  Psicologo = 'Psicologo',
  Nutricionista = 'Nutricionista',
  Fisioterapeuta = 'Fisioterapeuta',
  Dentista = 'Dentista',
  Enfermeiro = 'Enfermeiro',
  TerapeutaOcupacional = 'TerapeutaOcupacional',
  Fonoaudiologo = 'Fonoaudiologo',
  Outro = 'Outro'
}

@Injectable({
  providedIn: 'root'
})
export class TerminologyService {
  private readonly baseUrl = '/api/BusinessConfiguration';
  private readonly terminologyUrl = `${environment.apiUrl}/api/consultation-form-configurations/terminology`;
  private terminologyCache$ = new BehaviorSubject<TerminologyMap | null>(null);
  private specialtyTerminologyCache = new Map<string, Observable<TerminologyMap>>();
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
    const cached = this.terminologyCache$.value;
    if (cached) {
      return of(cached);
    }

    if (this.loading) {
      // Return existing cache observable instead of making duplicate requests
      return this.terminologyCache$.asObservable().pipe(
        filter(term => term !== null)
      ) as Observable<TerminologyMap>;
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
   * Get terminology for a specific professional specialty
   */
  getTerminologyBySpecialty(specialty?: string): Observable<TerminologyMap> {
    if (!specialty) {
      return of(this.defaultTerminology);
    }

    // Check cache first
    if (this.specialtyTerminologyCache.has(specialty)) {
      return this.specialtyTerminologyCache.get(specialty)!;
    }

    // Fetch from API and cache
    const request = this.http.get<TerminologyMap>(`${this.terminologyUrl}/${specialty}`)
      .pipe(
        catchError(() => of(this.defaultTerminology)),
        shareReplay(1)
      );

    this.specialtyTerminologyCache.set(specialty, request);
    return request;
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
    this.specialtyTerminologyCache.clear();
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

  /**
   * Parse specialty string to enum value (case-insensitive, handles various formats)
   */
  parseSpecialty(specialtyStr?: string): ProfessionalSpecialty {
    if (!specialtyStr) {
      return ProfessionalSpecialty.Outro;
    }

    const normalized = specialtyStr.toLowerCase().replace(/[^a-z]/g, '');
    
    switch (normalized) {
      case 'medico':
      case 'doctor':
      case 'doutor':
        return ProfessionalSpecialty.Medico;
      case 'psicologo':
      case 'psychologist':
        return ProfessionalSpecialty.Psicologo;
      case 'nutricionista':
      case 'nutritionist':
        return ProfessionalSpecialty.Nutricionista;
      case 'fisioterapeuta':
      case 'physiotherapist':
        return ProfessionalSpecialty.Fisioterapeuta;
      case 'dentista':
      case 'dentist':
      case 'odontologo':
        return ProfessionalSpecialty.Dentista;
      case 'enfermeiro':
      case 'nurse':
        return ProfessionalSpecialty.Enfermeiro;
      case 'terapeutaocupacional':
      case 'occupationaltherapist':
        return ProfessionalSpecialty.TerapeutaOcupacional;
      case 'fonoaudiologo':
      case 'speechtherapist':
        return ProfessionalSpecialty.Fonoaudiologo;
      default:
        return ProfessionalSpecialty.Outro;
    }
  }

  /**
   * Get the icon for a professional specialty
   */
  getSpecialtyIcon(specialty: ProfessionalSpecialty): string {
    switch (specialty) {
      case ProfessionalSpecialty.Medico:
        return '🩺';
      case ProfessionalSpecialty.Psicologo:
        return '🧠';
      case ProfessionalSpecialty.Nutricionista:
        return '🥗';
      case ProfessionalSpecialty.Fisioterapeuta:
        return '💪';
      case ProfessionalSpecialty.Dentista:
        return '🦷';
      case ProfessionalSpecialty.Enfermeiro:
        return '🩹';
      case ProfessionalSpecialty.TerapeutaOcupacional:
        return '🎨';
      case ProfessionalSpecialty.Fonoaudiologo:
        return '🗣️';
      default:
        return '⚕️';
    }
  }

  /**
   * Get translated specialty name (Portuguese)
   */
  getSpecialtyName(specialty: ProfessionalSpecialty): string {
    switch (specialty) {
      case ProfessionalSpecialty.Medico:
        return 'Médico';
      case ProfessionalSpecialty.Psicologo:
        return 'Psicólogo';
      case ProfessionalSpecialty.Nutricionista:
        return 'Nutricionista';
      case ProfessionalSpecialty.Fisioterapeuta:
        return 'Fisioterapeuta';
      case ProfessionalSpecialty.Dentista:
        return 'Dentista';
      case ProfessionalSpecialty.Enfermeiro:
        return 'Enfermeiro';
      case ProfessionalSpecialty.TerapeutaOcupacional:
        return 'Terapeuta Ocupacional';
      case ProfessionalSpecialty.Fonoaudiologo:
        return 'Fonoaudiólogo';
      default:
        return 'Outro';
    }
  }
}
