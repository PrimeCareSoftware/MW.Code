import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DigitalPrescription, PRESCRIPTION_TYPES } from '../../models/prescriptions/digital-prescription.model';

@Component({
  selector: 'app-digital-prescription-view',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="prescription-view" #prescriptionContent>
      <div class="prescription-header">
        <div class="logo-section">
          <h1>MedicWarehouse</h1>
          <p class="subtitle">Sistema de Prescrição Digital</p>
        </div>
        
        @if (prescription().verificationCode) {
          <div class="qr-code-section">
            <div class="qr-code-placeholder">
              <!-- QR Code seria gerado aqui com uma biblioteca -->
              <div class="qr-placeholder">
                <p>QR CODE</p>
                <small>{{ prescription().verificationCode }}</small>
              </div>
            </div>
            <small class="verification-code">
              Código: {{ prescription().verificationCode }}
            </small>
          </div>
        }
      </div>

      <!-- Prescription Type Badge -->
      <div class="prescription-type-badge" [style.background-color]="getTypeColor()">
        {{ getTypeName() }}
        @if (isControlled()) {
          <span class="controlled-icon">🔒</span>
        }
      </div>

      <!-- Status Badge -->
      <div class="status-section">
        <div class="status-badge" [class]="getStatusClass()">
          {{ getStatusText() }}
        </div>
        @if (prescription().isExpired) {
          <div class="warning-badge">
            ⚠️ Prescrição Expirada
          </div>
        }
        @if (prescription().daysUntilExpiration !== undefined && prescription().daysUntilExpiration! <= 3 && !prescription().isExpired) {
          <div class="warning-badge">
            ⚠️ Expira em {{ prescription().daysUntilExpiration }} dia(s)
          </div>
        }
      </div>

      <!-- Doctor Information -->
      <div class="info-section">
        <h3>Médico Prescritor</h3>
        <div class="info-row">
          <span class="label">Nome:</span>
          <span class="value">{{ prescription().doctorName }}</span>
        </div>
        <div class="info-row">
          <span class="label">CRM:</span>
          <span class="value">{{ prescription().doctorCRM }} / {{ prescription().doctorCRMState }}</span>
        </div>
      </div>

      <!-- Patient Information -->
      <div class="info-section">
        <h3>Paciente</h3>
        <div class="info-row">
          <span class="label">Nome:</span>
          <span class="value">{{ prescription().patientName }}</span>
        </div>
        <div class="info-row">
          <span class="label">CPF:</span>
          <span class="value">{{ prescription().patientDocument }}</span>
        </div>
      </div>

      <!-- Prescription Details -->
      <div class="info-section">
        <h3>Dados da Prescrição</h3>
        <div class="info-row">
          <span class="label">Data de Emissão:</span>
          <span class="value">{{ prescription().issuedAt | date: 'dd/MM/yyyy HH:mm' }}</span>
        </div>
        <div class="info-row">
          <span class="label">Validade:</span>
          <span class="value">{{ prescription().expiresAt | date: 'dd/MM/yyyy' }}</span>
        </div>
        @if (prescription().sequenceNumber) {
          <div class="info-row">
            <span class="label">Número Sequencial:</span>
            <span class="value">{{ prescription().sequenceNumber }}</span>
          </div>
        }
      </div>

      <!-- Medications -->
      <div class="medications-section">
        <h3>Medicamentos Prescritos</h3>
        @for (item of prescription().items; track $index) {
          <div class="medication-item">
            <div class="medication-header">
              <span class="item-number">{{ $index + 1 }}.</span>
              <h4>{{ item.medicationName }}</h4>
              @if (item.isControlledSubstance && item.controlledList) {
                <span class="controlled-badge">
                  🔒 Lista {{ item.controlledList }}
                </span>
              }
            </div>
            
            <div class="medication-details">
              <div class="detail-row">
                <strong>Apresentação:</strong> {{ item.pharmaceuticalForm }} - {{ item.dosage }}
              </div>
              @if (item.genericName) {
                <div class="detail-row">
                  <strong>Nome Genérico:</strong> {{ item.genericName }}
                </div>
              }
              <div class="detail-row">
                <strong>Posologia:</strong> {{ item.frequency }}
              </div>
              <div class="detail-row">
                <strong>Duração do Tratamento:</strong> {{ item.durationDays }} dias
              </div>
              <div class="detail-row">
                <strong>Quantidade:</strong> {{ item.quantity }} {{ item.pharmaceuticalForm }}(s)
              </div>
              @if (item.instructions) {
                <div class="detail-row instructions">
                  <strong>Instruções:</strong> {{ item.instructions }}
                </div>
              }
              @if (item.anvisaRegistration) {
                <div class="detail-row">
                  <small><strong>Registro ANVISA:</strong> {{ item.anvisaRegistration }}</small>
                </div>
              }
            </div>
          </div>
        }
      </div>

      <!-- Observations -->
      @if (prescription().notes) {
        <div class="observations-section">
          <h3>Observações</h3>
          <p>{{ prescription().notes }}</p>
        </div>
      }

      <!-- Digital Signature -->
      @if (prescription().digitalSignature) {
        <div class="signature-section">
          <h3>Assinatura Digital</h3>
          <div class="signature-info">
            <p><strong>Assinado em:</strong> {{ prescription().signedAt | date: 'dd/MM/yyyy HH:mm' }}</p>
            @if (prescription().signatureCertificate) {
              <p><strong>Certificado:</strong> {{ prescription().signatureCertificate }}</p>
            }
            <div class="signature-hash">
              <small>Hash: {{ prescription().digitalSignature?.substring(0, 64) }}...</small>
            </div>
          </div>
        </div>
      }

      <!-- SNGPC Notice -->
      @if (prescription().requiresSNGPCReport) {
        <div class="sngpc-notice">
          <h4>⚠️ Notificação SNGPC</h4>
          <p>Esta prescrição contém substâncias controladas e requer notificação ao Sistema Nacional de Gerenciamento de Produtos Controlados (SNGPC).</p>
          @if (prescription().reportedToSNGPCAt) {
            <p class="reported"><strong>✓ Reportado em:</strong> {{ prescription().reportedToSNGPCAt | date: 'dd/MM/yyyy HH:mm' }}</p>
          } @else {
            <p class="not-reported"><strong>⚠️ Pendente de reporte ao SNGPC</strong></p>
          }
        </div>
      }

      <!-- Legal Footer -->
      <div class="legal-footer">
        <p><small>
          Esta prescrição digital é válida em todo território nacional conforme Resolução CFM nº 1.643/2002.
          A autenticidade deste documento pode ser verificada através do código QR ou do código de verificação.
        </small></p>
        <p><small>
          Para medicamentos controlados, siga a Portaria ANVISA nº 344/1998 e suas atualizações.
        </small></p>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="action-buttons">
      <button
        type="button"
        class="btn btn-secondary"
        (click)="onClose()"
      >
        Fechar
      </button>
      @if (!prescription().digitalSignature) {
        <button
          type="button"
          class="btn btn-primary"
          (click)="onSign()"
        >
          🔒 Assinar Digitalmente
        </button>
      }
      <button
        type="button"
        class="btn btn-success"
        (click)="onPrint()"
      >
        🖨️ Imprimir
      </button>
      <button
        type="button"
        class="btn btn-info"
        (click)="onDownloadPDF()"
      >
        📄 Baixar PDF
      </button>
    </div>
  `,
  styles: [`
    .prescription-view {
      background: white;
      padding: 2rem;
      max-width: 900px;
      margin: 0 auto;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .prescription-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 2rem;
      padding-bottom: 1rem;
      border-bottom: 2px solid #007bff;
    }

    .logo-section h1 {
      color: #007bff;
      margin: 0 0 0.25rem 0;
      font-size: 2rem;
    }

    .logo-section .subtitle {
      color: #666;
      margin: 0;
      font-size: 0.9rem;
    }

    .qr-code-section {
      text-align: center;
    }

    .qr-placeholder {
      width: 100px;
      height: 100px;
      border: 2px solid #ddd;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      background: #f8f9fa;
    }

    .qr-placeholder p {
      margin: 0;
      font-size: 0.7rem;
      font-weight: bold;
      color: #666;
    }

    .qr-placeholder small {
      font-size: 0.6rem;
      color: #999;
    }

    .verification-code {
      display: block;
      margin-top: 0.5rem;
      font-size: 0.75rem;
      color: #666;
    }

    .prescription-type-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.75rem 1.5rem;
      border-radius: 6px;
      color: white;
      font-weight: 600;
      font-size: 1.1rem;
      margin-bottom: 1rem;
    }

    .controlled-icon {
      font-size: 1.2rem;
    }

    .status-section {
      margin-bottom: 2rem;
      display: flex;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .status-badge {
      padding: 0.5rem 1rem;
      border-radius: 4px;
      font-weight: 500;
      font-size: 0.9rem;
    }

    .status-badge.active {
      background: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }

    .status-badge.signed {
      background: #cce5ff;
      color: #004085;
      border: 1px solid #b8daff;
    }

    .status-badge.expired {
      background: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .warning-badge {
      padding: 0.5rem 1rem;
      background: #fff3cd;
      color: #856404;
      border: 1px solid #ffc107;
      border-radius: 4px;
      font-weight: 500;
      font-size: 0.9rem;
    }

    .info-section {
      margin-bottom: 2rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .info-section h3 {
      margin: 0 0 1rem 0;
      color: #333;
      font-size: 1.1rem;
      border-bottom: 1px solid #ddd;
      padding-bottom: 0.5rem;
    }

    .info-row {
      display: flex;
      margin-bottom: 0.5rem;
      font-size: 0.95rem;
    }

    .info-row .label {
      font-weight: 600;
      min-width: 150px;
      color: #555;
    }

    .info-row .value {
      color: #333;
    }

    .medications-section {
      margin-bottom: 2rem;
    }

    .medications-section h3 {
      margin-bottom: 1rem;
      color: #333;
      font-size: 1.2rem;
      border-bottom: 2px solid #28a745;
      padding-bottom: 0.5rem;
    }

    .medication-item {
      padding: 1.5rem;
      margin-bottom: 1rem;
      border: 2px solid #ddd;
      border-radius: 6px;
      background: #fff;
    }

    .medication-header {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 1rem;
    }

    .item-number {
      font-size: 1.3rem;
      font-weight: bold;
      color: #007bff;
    }

    .medication-header h4 {
      flex: 1;
      margin: 0;
      color: #333;
      font-size: 1.1rem;
    }

    .controlled-badge {
      padding: 0.25rem 0.75rem;
      background: #fff3cd;
      color: #856404;
      border: 1px solid #ffc107;
      border-radius: 12px;
      font-size: 0.8rem;
      font-weight: 600;
    }

    .medication-details {
      padding-left: 2rem;
    }

    .detail-row {
      margin-bottom: 0.5rem;
      font-size: 0.95rem;
      color: #555;
      line-height: 1.6;
    }

    .detail-row.instructions {
      margin-top: 0.75rem;
      padding: 0.75rem;
      background: #e7f3ff;
      border-left: 3px solid #007bff;
      border-radius: 4px;
    }

    .observations-section {
      margin-bottom: 2rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .observations-section h3 {
      margin: 0 0 0.75rem 0;
      color: #333;
      font-size: 1.1rem;
    }

    .observations-section p {
      margin: 0;
      color: #555;
      line-height: 1.6;
    }

    .signature-section {
      margin-bottom: 2rem;
      padding: 1rem;
      background: #e7f3ff;
      border: 2px solid #007bff;
      border-radius: 6px;
    }

    .signature-section h3 {
      margin: 0 0 0.75rem 0;
      color: #007bff;
      font-size: 1.1rem;
    }

    .signature-info p {
      margin: 0.5rem 0;
      color: #333;
    }

    .signature-hash {
      margin-top: 0.75rem;
      padding: 0.5rem;
      background: white;
      border-radius: 4px;
      font-family: 'Courier New', monospace;
    }

    .signature-hash small {
      word-break: break-all;
      color: #666;
    }

    .sngpc-notice {
      margin-bottom: 2rem;
      padding: 1rem;
      background: #fff3cd;
      border: 2px solid #ffc107;
      border-radius: 6px;
    }

    .sngpc-notice h4 {
      margin: 0 0 0.75rem 0;
      color: #856404;
      font-size: 1rem;
    }

    .sngpc-notice p {
      margin: 0.5rem 0;
      color: #856404;
      font-size: 0.9rem;
    }

    .sngpc-notice .reported {
      color: #155724;
      font-weight: 500;
    }

    .sngpc-notice .not-reported {
      color: #721c24;
      font-weight: 500;
    }

    .legal-footer {
      margin-top: 2rem;
      padding-top: 1rem;
      border-top: 1px solid #ddd;
      color: #666;
      font-size: 0.85rem;
    }

    .legal-footer p {
      margin: 0.5rem 0;
    }

    .action-buttons {
      display: flex;
      justify-content: center;
      gap: 1rem;
      margin-top: 2rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 1rem;
      font-weight: 500;
      transition: all 0.2s;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
    }

    .btn-secondary:hover {
      background: #5a6268;
    }

    .btn-primary {
      background: #007bff;
      color: white;
    }

    .btn-primary:hover {
      background: #0056b3;
    }

    .btn-success {
      background: #28a745;
      color: white;
    }

    .btn-success:hover {
      background: #218838;
    }

    .btn-info {
      background: #17a2b8;
      color: white;
    }

    .btn-info:hover {
      background: #138496;
    }

    @media print {
      .action-buttons {
        display: none;
      }

      .prescription-view {
        box-shadow: none;
      }
    }

    @media (max-width: 768px) {
      .prescription-header {
        flex-direction: column;
        gap: 1rem;
      }

      .action-buttons {
        flex-wrap: wrap;
      }

      .btn {
        flex: 1;
        min-width: 140px;
      }
    }
  `]
})
export class DigitalPrescriptionViewComponent implements OnInit {
  @Input() prescriptionData!: DigitalPrescription;
  @Output() closed = new EventEmitter<void>();
  @Output() signed = new EventEmitter<string>();
  @Output() printed = new EventEmitter<void>();
  @Output() downloadedPDF = new EventEmitter<void>();

  prescription = signal<DigitalPrescription>({} as DigitalPrescription);

  ngOnInit() {
    if (this.prescriptionData) {
      this.prescription.set(this.prescriptionData);
    }
  }

  getTypeName(): string {
    const type = PRESCRIPTION_TYPES.find(t => t.type === this.prescription().type);
    return type?.name || 'Receita';
  }

  getTypeColor(): string {
    const type = PRESCRIPTION_TYPES.find(t => t.type === this.prescription().type);
    const colorMap: Record<string, string> = {
      'primary': '#007bff',
      'warn': '#ff9800',
      'accent': '#e91e63'
    };
    return colorMap[type?.color || 'primary'] || '#007bff';
  }

  isControlled(): boolean {
    const type = PRESCRIPTION_TYPES.find(t => t.type === this.prescription().type);
    return type?.requiresSNGPC || false;
  }

  getStatusClass(): string {
    if (this.prescription().isExpired) return 'expired';
    if (this.prescription().digitalSignature) return 'signed';
    return 'active';
  }

  getStatusText(): string {
    if (this.prescription().isExpired) return 'Expirada';
    if (this.prescription().digitalSignature) return 'Assinada Digitalmente';
    return 'Ativa';
  }

  onClose() {
    this.closed.emit();
  }

  onSign() {
    // In production, this would integrate with ICP-Brasil or similar
    // For now, emit event for parent to handle
    this.signed.emit(this.prescription().id);
  }

  onPrint() {
    window.print();
    this.printed.emit();
  }

  onDownloadPDF() {
    // In production, this would generate a PDF
    // For now, just notify
    alert('Funcionalidade de download de PDF será implementada em breve.\nPor enquanto, use a opção de impressão do navegador.');
    this.downloadedPDF.emit();
  }
}
