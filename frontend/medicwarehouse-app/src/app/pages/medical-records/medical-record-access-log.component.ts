import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MedicalRecordService } from '../../services/medical-record';
import { MedicalRecordAccessLog } from '../../models/medical-record.model';

@Component({
  selector: 'app-medical-record-access-log',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './medical-record-access-log.component.html',
  styleUrls: ['./medical-record-access-log.component.scss']
})
export class MedicalRecordAccessLogComponent implements OnInit {
  @Input() medicalRecordId!: string;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  dataSource = new MatTableDataSource<MedicalRecordAccessLog>([]);
  displayedColumns: string[] = ['accessedAt', 'userName', 'accessType', 'ipAddress'];
  
  filterForm: FormGroup;
  loading = false;
  error: string | null = null;

  constructor(
    private medicalRecordService: MedicalRecordService,
    private fb: FormBuilder
  ) {
    this.filterForm = this.fb.group({
      startDate: [null],
      endDate: [null]
    });
  }

  ngOnInit(): void {
    if (!this.medicalRecordId) {
      this.error = 'ID do prontuário não fornecido';
      return;
    }
    this.loadAccessLogs();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadAccessLogs(): void {
    this.loading = true;
    this.error = null;

    const startDate = this.filterForm.get('startDate')?.value;
    const endDate = this.filterForm.get('endDate')?.value;

    this.medicalRecordService.getAccessLogs(
      this.medicalRecordId,
      startDate,
      endDate
    ).subscribe({
      next: (logs) => {
        this.dataSource.data = logs;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar logs de acesso';
        this.loading = false;
        console.error('Error loading access logs:', err);
      }
    });
  }

  applyFilter(): void {
    this.loadAccessLogs();
  }

  clearFilter(): void {
    this.filterForm.reset();
    this.loadAccessLogs();
  }

  refresh(): void {
    this.loadAccessLogs();
  }

  getAccessTypeIcon(accessType: string): string {
    const iconMap: Record<string, string> = {
      'View': 'visibility',
      'Edit': 'edit',
      'Close': 'lock',
      'Reopen': 'lock_open',
      'Print': 'print',
      'Export': 'download'
    };
    return iconMap[accessType] || 'info';
  }
}
