import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
// TODO: Install @angular/material to enable Material Design components
// import { MatCardModule } from '@angular/material/card';
// import { MatButtonModule } from '@angular/material/button';
// import { MatIconModule } from '@angular/material/icon';
// import { MatChipsModule } from '@angular/material/chips';
import { PRESCRIPTION_TYPES, PrescriptionType, PrescriptionTypeInfo } from '../../../models/prescriptions/digital-prescription.model';

/**
 * Component for selecting prescription type (CFM 1.643/2002 + ANVISA 344/1998)
 * Displays available prescription types with their characteristics
 * NOTE: This component requires Angular Material to be installed for full functionality
 */
@Component({
  selector: 'app-prescription-type-selector',
  standalone: true,
  imports: [
    CommonModule,
    // TODO: Uncomment when Material is installed
    // MatCardModule,
    // MatButtonModule,
    // MatIconModule,
    // MatChipsModule
  ],
  templateUrl: './prescription-type-selector.component.html',
  styleUrl: './prescription-type-selector.component.scss'})
export class PrescriptionTypeSelectorComponent {
  @Output() typeSelected = new EventEmitter<PrescriptionType>();

  prescriptionTypes = PRESCRIPTION_TYPES;
  showControlledWarning = true;

  selectType(typeInfo: PrescriptionTypeInfo): void {
    this.typeSelected.emit(typeInfo.type);
  }
}
