import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  imports: [CommonModule, RouterLink],
  templateUrl: './forbidden.html',
  styleUrl: './forbidden.scss'
})
export class Forbidden {
}
