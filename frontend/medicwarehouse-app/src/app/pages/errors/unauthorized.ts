import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  imports: [CommonModule],
  templateUrl: './unauthorized.html',
  styleUrl: './unauthorized.scss'
})
export class Unauthorized implements OnInit {
  returnUrl: string = '/dashboard';

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    // Get return URL from query parameters or default to dashboard
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
  }

  navigateToLogin() {
    // If the return URL is a system-admin route, redirect to system-admin login
    const isSystemAdminRoute = this.returnUrl.startsWith('/system-admin');
    const loginPath = isSystemAdminRoute ? '/system-admin/login' : '/login';
    
    this.router.navigate([loginPath], { queryParams: { returnUrl: this.returnUrl } });
  }
}
