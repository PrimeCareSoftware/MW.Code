import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="error-container">
      <h1>403 - Acesso Negado</h1>
      <p>Você não tem permissão para acessar esta página.</p>
      <p>Esta área é restrita a administradores do sistema.</p>
      <a routerLink="/login" class="btn">Voltar ao Login</a>
    </div>
  `,
  styles: [`
    .error-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 100vh;
      text-align: center;
      padding: 2rem;
    }
    
    h1 {
      font-size: 3rem;
      margin-bottom: 1rem;
      color: #e74c3c;
    }
    
    p {
      font-size: 1.2rem;
      margin-bottom: 0.5rem;
      color: #666;
    }
    
    .btn {
      margin-top: 2rem;
      padding: 0.75rem 1.5rem;
      background-color: #3498db;
      color: white;
      text-decoration: none;
      border-radius: 4px;
      transition: background-color 0.3s;
    }
    
    .btn:hover {
      background-color: #2980b9;
    }
  `]
})
export class Forbidden {}
