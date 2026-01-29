import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { WorkflowService } from '../../services/workflow.service';
import { WorkflowExecution, WorkflowDto } from '../../models/workflow.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-workflow-executions',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './workflow-executions.html',
  styleUrl: './workflow-executions.scss'
})
export class WorkflowExecutions implements OnInit {
  workflowId!: number;
  workflow = signal<WorkflowDto | null>(null);
  executions = signal<WorkflowExecution[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  currentPage = signal(1);
  pageSize = 20;

  constructor(
    private workflowService: WorkflowService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.workflowId = parseInt(id);
      this.loadWorkflow();
      this.loadExecutions();
    }
  }

  loadWorkflow(): void {
    this.workflowService.getById(this.workflowId).subscribe({
      next: (workflow) => {
        this.workflow.set(workflow);
      },
      error: (err) => {
        console.error('Error loading workflow:', err);
      }
    });
  }

  loadExecutions(): void {
    this.loading.set(true);
    this.error.set(null);

    this.workflowService.getExecutions(this.workflowId, this.currentPage(), this.pageSize).subscribe({
      next: (data) => {
        this.executions.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error loading executions');
        this.loading.set(false);
      }
    });
  }

  viewExecutionDetails(executionId: number): void {
    this.workflowService.getExecution(executionId).subscribe({
      next: (execution) => {
        const message = `
Execution #${execution.id}
Status: ${execution.status}
Started: ${new Date(execution.startedAt).toLocaleString()}
${execution.completedAt ? 'Completed: ' + new Date(execution.completedAt).toLocaleString() : ''}
${execution.error ? 'Error: ' + execution.error : ''}

Actions:
${execution.actionExecutions?.map(ae => 
  `- ${ae.workflowAction?.actionType}: ${ae.status}${ae.error ? ' (' + ae.error + ')' : ''}`
).join('\n') || 'No actions'}
        `;
        alert(message);
      },
      error: (err) => {
        alert(err.error?.message || 'Error loading execution details');
      }
    });
  }

  getStatusClass(status: string): string {
    return status;
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'completed': return '✅';
      case 'failed': return '❌';
      case 'running': return '⏳';
      case 'pending': return '⏸️';
      default: return '❓';
    }
  }

  getDuration(execution: WorkflowExecution): string {
    if (!execution.completedAt) {
      return 'In progress';
    }
    const start = new Date(execution.startedAt).getTime();
    const end = new Date(execution.completedAt).getTime();
    const seconds = Math.floor((end - start) / 1000);
    if (seconds < 60) return `${seconds}s`;
    const minutes = Math.floor(seconds / 60);
    return `${minutes}m ${seconds % 60}s`;
  }

  backToWorkflows(): void {
    this.router.navigate(['/workflows']);
  }
}
