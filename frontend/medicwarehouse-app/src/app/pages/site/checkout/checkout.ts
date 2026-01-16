import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-checkout',
  imports: [CommonModule, RouterLink],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss'
})
export class CheckoutComponent implements OnInit {
  private route = inject(ActivatedRoute);
  
  success = false;
  clinicId = '';
  userId = '';
  tenantId = '';
  subdomain = '';
  clinicName = '';
  ownerName = '';
  ownerEmail = '';
  username = '';
  
  // Use environment URL for app link
  readonly appUrl = environment.appUrl;

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.success = params['success'] === 'true';
      this.clinicId = params['clinicId'] || '';
      this.userId = params['userId'] || '';
      this.tenantId = params['tenantId'] || '';
      this.subdomain = params['subdomain'] || '';
      this.clinicName = params['clinicName'] || '';
      this.ownerName = params['ownerName'] || '';
      this.ownerEmail = params['ownerEmail'] || '';
      this.username = params['username'] || '';
    });
  }
}
