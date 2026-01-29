import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { WorkflowService } from '../../services/workflow.service';
import { WorkflowDto } from '../../models/workflow.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-workflows-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './workflows-list.html',
  styleUrl: './workflows-list.scss'
})
export class WorkflowsList implements OnInit {
  workflows = signal<WorkflowDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  constructor(
    private workflowService: WorkflowService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadWorkflows();
  }

  loadWorkflows(): void {
    this.loading.set(true);
    this.error.set(null);

    this.workflowService.getAll().subscribe({
      next: (data) => {
        this.workflows.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error loading workflows');
        this.loading.set(false);
      }
    });
  }

  toggleWorkflow(id: number): void {
    this.workflowService.toggle(id).subscribe({
      next: () => {
        this.loadWorkflows();
      },
      error: (err) => {
        alert(err.error?.message || 'Error toggling workflow');
      }
    });
  }

  deleteWorkflow(id: number, name: string): void {
    if (!confirm(`Are you sure you want to delete workflow "${name}"?`)) {
      return;
    }

    this.workflowService.delete(id).subscribe({
      next: () => {
        this.loadWorkflows();
      },
      error: (err) => {
        alert(err.error?.message || 'Error deleting workflow');
      }
    });
  }

  createWorkflow(): void {
    this.router.navigate(['/workflows/create']);
  }

  editWorkflow(id: number): void {
    this.router.navigate(['/workflows', id, 'edit']);
  }

  viewExecutions(id: number): void {
    this.router.navigate(['/workflows', id, 'executions']);
  }

  testWorkflow(id: number): void {
    const testData = { test: true };
    this.workflowService.test(id, testData).subscribe({
      next: (execution) => {
        alert(`Workflow test completed with status: ${execution.status}`);
        this.viewExecutions(id);
      },
      error: (err) => {
        alert(err.error?.message || 'Error testing workflow');
      }
    });
  }

  getActionTypesDisplay(workflow: WorkflowDto): string {
    if (!workflow.actions || workflow.actions.length === 0) {
      return 'No actions';
    }
    const types = [...new Set(workflow.actions.map(a => a.actionType))];
    return types.join(', ');
  }

  getActionCount(workflow: WorkflowDto): number {
    return workflow.actions?.length || 0;
  }
}
