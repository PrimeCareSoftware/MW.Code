import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DigitalPrescription, PRESCRIPTION_TYPES } from '../../models/prescriptions/digital-prescription.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-digital-prescription-view',
  standalone: true,
  imports: [CommonModule, Navbar],
  templateUrl: './digital-prescription-view.component.html',
  styleUrl: './digital-prescription-view.component.scss'})
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
