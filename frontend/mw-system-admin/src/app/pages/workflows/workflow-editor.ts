import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { WorkflowService } from '../../services/workflow.service';
import { 
  WorkflowDto, 
  CreateWorkflowDto, 
  CreateWorkflowActionDto,
  TriggerTypes,
  ActionTypes,
  EventTypes
} from '../../models/workflow.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-workflow-editor',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './workflow-editor.html',
  styleUrl: './workflow-editor.scss'
})
export class WorkflowEditor implements OnInit {
  workflowId: number | null = null;
  loading = signal(true);
  saving = signal(false);
  error = signal<string | null>(null);
  
  formData: CreateWorkflowDto = {
    name: '',
    description: '',
    isEnabled: true,
    triggerType: TriggerTypes.EVENT,
    triggerConfig: '',
    stopOnError: false,
    actions: []
  };

  actions: CreateWorkflowActionDto[] = [];

  // Available options
  triggerTypes = [
    { value: TriggerTypes.TIME, label: 'Time-based (Schedule)' },
    { value: TriggerTypes.EVENT, label: 'Event-based' },
    { value: TriggerTypes.MANUAL, label: 'Manual Trigger' }
  ];

  eventTypes = Object.entries(EventTypes).map(([key, value]) => ({
    value,
    label: key.replace(/_/g, ' ').toLowerCase().replace(/\b\w/g, c => c.toUpperCase())
  }));

  actionTypes = [
    { value: ActionTypes.EMAIL, label: 'Send Email', icon: '📧' },
    { value: ActionTypes.SMS, label: 'Send SMS', icon: '📱' },
    { value: ActionTypes.WEBHOOK, label: 'Call Webhook', icon: '🔗' },
    { value: ActionTypes.TAG, label: 'Add Tag', icon: '🏷️' },
    { value: ActionTypes.TICKET, label: 'Create Ticket', icon: '🎫' },
    { value: ActionTypes.SLACK, label: 'Send Slack Message', icon: '💬' },
    { value: ActionTypes.UPDATE_FIELD, label: 'Update Field', icon: '✏️' }
  ];

  constructor(
    private workflowService: WorkflowService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'create') {
      this.workflowId = parseInt(id);
      this.loadWorkflow(this.workflowId);
    } else {
      this.loading.set(false);
    }
  }

  loadWorkflow(id: number): void {
    this.workflowService.getById(id).subscribe({
      next: (workflow) => {
        this.formData = {
          name: workflow.name,
          description: workflow.description || '',
          isEnabled: workflow.isEnabled,
          triggerType: workflow.triggerType,
          triggerConfig: workflow.triggerConfig || '',
          stopOnError: workflow.stopOnError,
          actions: workflow.actions || []
        };
        this.actions = [...(workflow.actions || [])];
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error loading workflow');
        this.loading.set(false);
      }
    });
  }

  addAction(): void {
    this.actions.push({
      order: this.actions.length,
      actionType: ActionTypes.EMAIL,
      config: '{}',
      condition: '',
      delaySeconds: 0
    });
  }

  removeAction(index: number): void {
    this.actions.splice(index, 1);
    this.reorderActions();
  }

  moveActionUp(index: number): void {
    if (index > 0) {
      const temp = this.actions[index];
      this.actions[index] = this.actions[index - 1];
      this.actions[index - 1] = temp;
      this.reorderActions();
    }
  }

  moveActionDown(index: number): void {
    if (index < this.actions.length - 1) {
      const temp = this.actions[index];
      this.actions[index] = this.actions[index + 1];
      this.actions[index + 1] = temp;
      this.reorderActions();
    }
  }

  reorderActions(): void {
    this.actions.forEach((action, index) => {
      action.order = index;
    });
  }

  getActionIcon(actionType: string): string {
    const action = this.actionTypes.find(a => a.value === actionType);
    return action?.icon || '⚙️';
  }

  getActionLabel(actionType: string): string {
    const action = this.actionTypes.find(a => a.value === actionType);
    return action?.label || actionType;
  }

  onSubmit(): void {
    if (!this.formData.name) {
      alert('Workflow name is required');
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    const dto: CreateWorkflowDto = {
      ...this.formData,
      actions: this.actions
    };

    const request = this.workflowId 
      ? this.workflowService.update(this.workflowId, dto)
      : this.workflowService.create(dto);

    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/workflows']);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Error saving workflow');
        this.saving.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/workflows']);
  }
}
