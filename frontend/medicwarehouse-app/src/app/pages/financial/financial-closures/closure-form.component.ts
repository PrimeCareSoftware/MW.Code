import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-closure-form',
  imports: [CommonModule, Navbar],
  templateUrl: './closure-form.component.html',
  styleUrl: './closure-form.component.scss'
})
export class ClosureFormComponent {}
