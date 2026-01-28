import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../../shared/navbar/navbar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-complaint-list',
  imports: [CommonModule, Navbar, FormsModule],
  templateUrl: './complaint-list.html',
  styleUrl: './complaint-list.scss'
})
export class ComplaintList implements OnInit {
  complaints = signal<any[]>([]);
  isLoading = signal<boolean>(false);

  constructor() {}

  ngOnInit(): void {
    this.loadComplaints();
  }

  async loadComplaints(): Promise<void> {
    this.isLoading.set(true);
    try {
      // TODO: Integrate with ComplaintService when API is connected
      // For now, just set empty array
      this.complaints.set([]);
    } catch (error) {
      console.error('Error loading complaints:', error);
    } finally {
      this.isLoading.set(false);
    }
  }
}
