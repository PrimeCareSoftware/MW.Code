import { Pipe, PipeTransform } from '@angular/core';
import { TerminologyService, TerminologyMap } from '../services/terminology.service';

@Pipe({
  name: 'terminology',
  standalone: true,
  pure: false // Make it impure to react to terminology changes
})
export class TerminologyPipe implements PipeTransform {
  constructor(private terminologyService: TerminologyService) {}

  transform(key: keyof TerminologyMap | string, fallback?: string): string {
    if (!key) {
      return fallback || '';
    }

    // If the key contains placeholders, replace them
    if (key.includes('{{')) {
      return this.terminologyService.replacePlaceholders(key);
    }

    // Otherwise, treat it as a direct key lookup
    return this.terminologyService.getTerm(key as keyof TerminologyMap) || fallback || key;
  }
}
