import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-site-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './site-layout.html',
  styleUrl: './site-layout.scss'
})
export class SiteLayoutComponent {}
