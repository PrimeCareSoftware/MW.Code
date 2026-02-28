import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplaintService } from '../../../services/crm';
import { Complaint } from '../../../models/crm';

@Component({
  selector: 'app-complaint-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './complaint-list.html',
  styleUrl: './complaint-list.scss'
})
export class ComplaintList implements OnInit {
  complaints = signal<Complaint[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private complaintService: ComplaintService) {}

  ngOnInit(): void {
    this.loadComplaints();
  }

  loadComplaints(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.complaintService.getAll().subscribe({
      next: (complaints) => {
        this.complaints.set(complaints);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading complaints:', error);
        this.errorMessage.set(error.userMessage || error.message || 'Erro ao carregar reclamações');
        this.complaints.set([]);
        this.isLoading.set(false);
      }
    });
  }

  onNewComplaint(): void {
    console.log('Nova Reclamação clicked');
    alert('Funcionalidade "Nova Reclamação" será implementada em breve.');
  }
}
