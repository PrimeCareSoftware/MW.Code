import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-closure-form',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './closure-form.component.html',
  styleUrl: './closure-form.component.scss'
})
export class ClosureFormComponent {}
