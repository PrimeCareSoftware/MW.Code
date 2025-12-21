import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export interface CepResponse {
  cep: string;
  logradouro: string;
  complemento: string;
  bairro: string;
  localidade: string;
  uf: string;
  erro?: boolean;
}

export interface AddressData {
  street: string;
  neighborhood: string;
  city: string;
  state: string;
  complement?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CepService {
  private readonly VIACEP_URL = 'https://viacep.com.br/ws';

  constructor(private http: HttpClient) {}

  /**
   * Lookup CEP using ViaCEP API
   * @param cep The CEP to lookup (can be formatted or not)
   * @returns Observable with address data or null if not found
   */
  lookupCep(cep: string): Observable<AddressData | null> {
    // Remove non-numeric characters
    const cleanCep = cep.replace(/\D/g, '');

    // Validate CEP length
    if (cleanCep.length !== 8) {
      return of(null);
    }

    return this.http.get<CepResponse>(`${this.VIACEP_URL}/${cleanCep}/json/`).pipe(
      map(response => {
        // Check if CEP was not found
        if (response.erro) {
          return null;
        }

        return {
          street: response.logradouro,
          neighborhood: response.bairro,
          city: response.localidade,
          state: response.uf,
          complement: response.complemento || undefined
        };
      }),
      catchError(error => {
        console.error('Error looking up CEP:', error);
        return of(null);
      })
    );
  }
}
