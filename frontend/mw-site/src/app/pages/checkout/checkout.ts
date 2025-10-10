import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

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

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.success = params['success'] === 'true';
      this.clinicId = params['clinicId'] || '';
    });
  }
}
