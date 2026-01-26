import { Component, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HelpService } from '../../services/help.service';
import { HelpDialogComponent } from '../../pages/help/help-dialog';

@Component({
  selector: 'app-help-button',
  standalone: true,
  imports: [CommonModule, HelpDialogComponent],
  templateUrl: './help-button.html',
  styleUrls: ['./help-button.scss']
})
export class HelpButtonComponent {
  @Input() module: string = '';
  @Input() position: 'top-right' | 'bottom-right' = 'bottom-right';
  
  @ViewChild(HelpDialogComponent) helpDialog!: HelpDialogComponent;

  constructor(private helpService: HelpService) {}

  openHelp(): void {
    const content = this.helpService.getHelpContent(this.module);
    if (content) {
      this.helpDialog.open(content);
    }
  }
}
