import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { PRESCRIPTION_TYPES, PrescriptionType, PrescriptionTypeInfo } from '../../../models/prescriptions/digital-prescription.model';

/**
 * Component for selecting prescription type (CFM 1.643/2002 + ANVISA 344/1998)
 * Displays available prescription types with their characteristics
 */
@Component({
  selector: 'app-prescription-type-selector',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  template: `
    <div class="prescription-type-selector">
      <h2>Selecione o Tipo de Receita</h2>
      <p class="subtitle">Conforme CFM 1.643/2002 e Portaria ANVISA 344/1998</p>

      <div class="prescription-types">
        <mat-card 
          *ngFor="let typeInfo of prescriptionTypes"
          class="prescription-type-card"
          [class.controlled]="typeInfo.requiresSNGPC"
          (click)="selectType(typeInfo)">
          
          <mat-card-header>
            <mat-icon [color]="typeInfo.color" class="type-icon">{{typeInfo.icon}}</mat-icon>
            <mat-card-title>{{ typeInfo.name }}</mat-card-title>
          </mat-card-header>

          <mat-card-content>
            <p class="description">{{ typeInfo.description }}</p>
            
            <div class="type-details">
              <mat-chip-set>
                <mat-chip>
                  <mat-icon>schedule</mat-icon>
                  Validade: {{ typeInfo.expirationDays }} dias
                </mat-chip>
                
                <mat-chip *ngIf="typeInfo.requiresSNGPC" class="sngpc-chip">
                  <mat-icon>report</mat-icon>
                  Requer SNGPC
                </mat-chip>
                
                <mat-chip *ngIf="typeInfo.requiresSpecialForm" class="special-form-chip">
                  <mat-icon>verified_user</mat-icon>
                  Formulário Especial
                </mat-chip>
              </mat-chip-set>
            </div>
          </mat-card-content>

          <mat-card-actions>
            <button mat-raised-button [color]="typeInfo.color">
              Selecionar
            </button>
          </mat-card-actions>
        </mat-card>
      </div>

      <div class="info-panel" *ngIf="showControlledWarning">
        <mat-icon class="warning-icon">warning</mat-icon>
        <div class="warning-content">
          <h3>Atenção: Medicamentos Controlados</h3>
          <ul>
            <li>Receitas de controle especial requerem numeração sequencial</li>
            <li>Obrigatória notificação ao SNGPC (Sistema Nacional de Gerenciamento de Produtos Controlados)</li>
            <li>Retenção obrigatória da receita pela farmácia</li>
            <li>Receitas tipo A requerem 2 vias</li>
          </ul>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .prescription-type-selector {
      padding: 20px;
    }

    h2 {
      color: #333;
      margin-bottom: 8px;
    }

    .subtitle {
      color: #666;
      font-size: 14px;
      margin-bottom: 24px;
    }

    .prescription-types {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
      gap: 20px;
      margin-bottom: 24px;
    }

    .prescription-type-card {
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .prescription-type-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    }

    .prescription-type-card.controlled {
      border-left: 4px solid #ff9800;
    }

    .type-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
      margin-right: 12px;
    }

    mat-card-header {
      display: flex;
      align-items: center;
      margin-bottom: 16px;
    }

    .description {
      color: #555;
      font-size: 14px;
      margin-bottom: 16px;
      line-height: 1.5;
    }

    .type-details {
      margin-top: 16px;
    }

    mat-chip {
      display: inline-flex;
      align-items: center;
      gap: 4px;
    }

    mat-chip mat-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
    }

    .sngpc-chip {
      background-color: #ff9800 !important;
      color: white !important;
    }

    .special-form-chip {
      background-color: #2196f3 !important;
      color: white !important;
    }

    .info-panel {
      display: flex;
      align-items: flex-start;
      gap: 16px;
      padding: 16px;
      background-color: #fff3cd;
      border-left: 4px solid #ff9800;
      border-radius: 4px;
    }

    .warning-icon {
      color: #ff9800;
      font-size: 32px;
      width: 32px;
      height: 32px;
    }

    .warning-content h3 {
      margin: 0 0 12px 0;
      color: #856404;
    }

    .warning-content ul {
      margin: 0;
      padding-left: 20px;
      color: #856404;
    }

    .warning-content li {
      margin-bottom: 8px;
    }
  `]
})
export class PrescriptionTypeSelectorComponent {
  @Output() typeSelected = new EventEmitter<PrescriptionType>();

  prescriptionTypes = PRESCRIPTION_TYPES;
  showControlledWarning = true;

  selectType(typeInfo: PrescriptionTypeInfo): void {
    this.typeSelected.emit(typeInfo.type);
  }
}
