import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OfflineService } from '../../../services/offline.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-offline-indicator',
  templateUrl: './offline-indicator.html',
  standalone: true,
  imports: [CommonModule],
  styleUrls: ['./offline-indicator.scss']
})
export class OfflineIndicatorComponent {
  isOnline$: Observable<boolean>;
  
  constructor(private offlineService: OfflineService) {
    this.isOnline$ = this.offlineService.isOnline$;
  }
}
