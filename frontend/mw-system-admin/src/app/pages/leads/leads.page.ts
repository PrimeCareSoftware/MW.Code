import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { LeadService } from '../../services/lead.service';
import {
  Lead,
  LeadActivity,
  LeadStatistics,
  LeadStatus,
  ActivityType,
  getLeadStatusLabel,
  getLeadStatusColor,
  getLeadScoreColor,
  getActivityTypeLabel
} from '../../models/lead.model';

@Component({
  selector: 'app-leads',
  templateUrl: './leads.page.html',
  styleUrls: ['./leads.page.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, IonicModule]
})
export class LeadsPage implements OnInit {
  leads: Lead[] = [];
  filteredLeads: Lead[] = [];
  statistics: LeadStatistics | null = null;
  
  // Filters
  statusFilter: LeadStatus | 'all' = 'all';
  assignmentFilter: 'all' | 'assigned' | 'unassigned' = 'all';
  searchTerm: string = '';
  
  // UI State
  loading = false;
  selectedLead: Lead | null = null;
  selectedLeadActivities: LeadActivity[] = [];
  showActivityModal = false;
  showAssignModal = false;
  showFollowUpModal = false;
  showNotesModal = false;
  
  // Modal data
  newActivity: {
    type: ActivityType;
    title: string;
    description: string;
    durationMinutes?: number;
    outcome: string;
  } = {
    type: ActivityType.Note,
    title: '',
    description: '',
    outcome: ''
  };
  
  assignUserId = '';
  followUpDate = '';
  newNotes = '';
  newStatus: LeadStatus = LeadStatus.New;
  statusNotes = '';
  
  // Enums for template
  LeadStatus = LeadStatus;
  ActivityType = ActivityType;
  
  constructor(private leadService: LeadService) {}

  ngOnInit() {
    this.loadData();
  }

  async loadData() {
    this.loading = true;
    try {
      await Promise.all([
        this.loadLeads(),
        this.loadStatistics()
      ]);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      this.loading = false;
    }
  }

  async loadLeads() {
    try {
      const leads = await this.leadService.getUnassignedLeads().toPromise();
      this.leads = leads || [];
      this.applyFilters();
    } catch (error) {
      console.error('Error loading leads:', error);
    }
  }

  async loadStatistics() {
    try {
      this.statistics = await this.leadService.getStatistics().toPromise();
    } catch (error) {
      console.error('Error loading statistics:', error);
    }
  }

  applyFilters() {
    this.filteredLeads = this.leads.filter(lead => {
      if (this.statusFilter !== 'all' && lead.status !== this.statusFilter) {
        return false;
      }
      
      if (this.assignmentFilter === 'assigned' && !lead.assignedToUserId) {
        return false;
      }
      if (this.assignmentFilter === 'unassigned' && lead.assignedToUserId) {
        return false;
      }
      
      if (this.searchTerm) {
        const search = this.searchTerm.toLowerCase();
        return (
          (lead.contactName?.toLowerCase().includes(search)) ||
          (lead.email?.toLowerCase().includes(search)) ||
          (lead.phone?.includes(search)) ||
          (lead.companyName?.toLowerCase().includes(search))
        );
      }
      
      return true;
    });
  }

  onStatusFilterChange() {
    this.applyFilters();
  }

  onAssignmentFilterChange() {
    this.applyFilters();
  }

  onSearchChange() {
    this.applyFilters();
  }

  async selectLead(lead: Lead) {
    this.selectedLead = lead;
    await this.loadLeadActivities(lead.id);
  }

  async loadLeadActivities(leadId: string) {
    try {
      this.selectedLeadActivities = await this.leadService.getLeadActivities(leadId).toPromise() || [];
    } catch (error) {
      console.error('Error loading activities:', error);
    }
  }

  async updateStatus(lead: Lead, newStatus: LeadStatus) {
    try {
      await this.leadService.updateLeadStatus(lead.id, { status: newStatus }).toPromise();
      lead.status = newStatus;
      await this.loadStatistics();
    } catch (error) {
      console.error('Error updating status:', error);
    }
  }

  openAssignModal(lead: Lead) {
    this.selectedLead = lead;
    this.assignUserId = '';
    this.showAssignModal = true;
  }

  async submitAssignment() {
    if (!this.selectedLead || !this.assignUserId) return;
    
    try {
      await this.leadService.assignLead(this.selectedLead.id, { userId: this.assignUserId }).toPromise();
      await this.loadLeads();
      this.showAssignModal = false;
      this.selectedLead = null;
    } catch (error) {
      console.error('Error assigning lead:', error);
    }
  }

  openFollowUpModal(lead: Lead) {
    this.selectedLead = lead;
    this.followUpDate = '';
    this.showFollowUpModal = true;
  }

  async submitFollowUp() {
    if (!this.selectedLead || !this.followUpDate) return;
    
    try {
      await this.leadService.scheduleFollowUp(this.selectedLead.id, { 
        followUpDate: new Date(this.followUpDate) 
      }).toPromise();
      
      if (this.selectedLead) {
        this.selectedLead.nextFollowUpDate = new Date(this.followUpDate);
      }
      this.showFollowUpModal = false;
      this.selectedLead = null;
    } catch (error) {
      console.error('Error scheduling follow-up:', error);
    }
  }

  openActivityModal(lead: Lead) {
    this.selectedLead = lead;
    this.newActivity = {
      type: ActivityType.Note,
      title: '',
      description: '',
      outcome: ''
    };
    this.showActivityModal = true;
  }

  async submitActivity() {
    if (!this.selectedLead || !this.newActivity.title) return;
    
    try {
      await this.leadService.addActivity(this.selectedLead.id, {
        type: this.newActivity.type,
        title: this.newActivity.title,
        description: this.newActivity.description || undefined,
        durationMinutes: this.newActivity.durationMinutes,
        outcome: this.newActivity.outcome || undefined
      }).toPromise();
      
      await this.loadLeadActivities(this.selectedLead.id);
      this.showActivityModal = false;
    } catch (error) {
      console.error('Error adding activity:', error);
    }
  }

  openNotesModal(lead: Lead) {
    this.selectedLead = lead;
    this.newNotes = '';
    this.showNotesModal = true;
  }

  async submitNotes() {
    if (!this.selectedLead || !this.newNotes) return;
    
    try {
      await this.leadService.addNotes(this.selectedLead.id, { notes: this.newNotes }).toPromise();
      this.showNotesModal = false;
      this.selectedLead = null;
    } catch (error) {
      console.error('Error adding notes:', error);
    }
  }

  getStatusLabel(status: LeadStatus): string {
    return getLeadStatusLabel(status);
  }

  getStatusColor(status: LeadStatus): string {
    return getLeadStatusColor(status);
  }

  getScoreColor(score: number): string {
    return getLeadScoreColor(score);
  }

  getActivityTypeLabel(type: ActivityType): string {
    return getActivityTypeLabel(type);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR', { 
      day: '2-digit', 
      month: '2-digit', 
      year: 'numeric' 
    });
  }

  formatDateTime(date: Date | string | undefined): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleString('pt-BR', { 
      day: '2-digit', 
      month: '2-digit', 
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
