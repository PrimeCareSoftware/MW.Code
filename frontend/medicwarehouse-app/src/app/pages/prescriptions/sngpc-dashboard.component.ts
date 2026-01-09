import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DigitalPrescriptionService } from '../../services/prescriptions/digital-prescription.service';
import { SNGPCReport, SNGPCReportStatus } from '../../models/prescriptions/digital-prescription.model';

/**
 * Dashboard for SNGPC (Sistema Nacional de Gerenciamento de Produtos Controlados) reporting
 * Shows report status, deadlines, and allows report generation and transmission
 * NOTE: This component requires Angular Material to be installed for full functionality
 */
@Component({
  selector: 'app-sngpc-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sngpc-dashboard.component.html',
  styleUrl: './sngpc-dashboard.component.scss'})
export class SNGPCDashboardComponent implements OnInit {
  private prescriptionService = inject(DigitalPrescriptionService);

  reports: SNGPCReport[] = [];
  overdueReports: SNGPCReport[] = [];
  latestReport: SNGPCReport | null = null;
  unreportedCount = 0;
  transmittedThisYear = 0;

  displayedColumns = ['period', 'status', 'prescriptions', 'items', 'deadline', 'actions'];

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    const currentYear = new Date().getFullYear();

    // Load report history
    this.prescriptionService.getSNGPCReportHistory(12).subscribe(reports => {
      this.reports = reports;
      this.transmittedThisYear = reports.filter(r => 
        r.status === SNGPCReportStatus.Transmitted && r.year === currentYear
      ).length;
    });

    // Load overdue reports
    this.prescriptionService.getOverdueSNGPCReports().subscribe(reports => {
      this.overdueReports = reports;
    });

    // Load latest report
    this.prescriptionService.getLatestSNGPCReport().subscribe(report => {
      this.latestReport = report;
    });

    // Load unreported prescriptions count
    this.prescriptionService.getUnreportedToSNGPC().subscribe(prescriptions => {
      this.unreportedCount = prescriptions.length;
    });
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      'Draft': 'Rascunho',
      'Generated': 'XML Gerado',
      'Transmitted': 'Transmitido',
      'TransmissionFailed': 'Falha na Transmissão',
      'Validated': 'Validado'
    };
    return labels[status] || status;
  }

  createNewReport(): void {
    const now = new Date();
    const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1);
    
    this.prescriptionService.createSNGPCReport({
      month: lastMonth.getMonth() + 1,
      year: lastMonth.getFullYear()
    }).subscribe(() => {
      this.loadDashboardData();
    });
  }

  generateXML(reportId: string): void {
    this.prescriptionService.generateSNGPCXML(reportId).subscribe(() => {
      this.loadDashboardData();
    });
  }

  downloadXML(reportId: string): void {
    this.prescriptionService.downloadSNGPCXML(reportId).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `SNGPC_${reportId}.xml`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  transmitReport(reportId: string): void {
    const protocol = `ANVISA-${Date.now()}`;
    this.prescriptionService.markSNGPCAsTransmitted(reportId, protocol).subscribe(() => {
      this.loadDashboardData();
    });
  }
}
