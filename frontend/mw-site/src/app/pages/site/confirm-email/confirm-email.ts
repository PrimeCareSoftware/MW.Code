import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-confirm-email',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './confirm-email.html',
  styleUrl: './confirm-email.scss'
})
export class ConfirmEmailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);

  status = signal<'loading' | 'success' | 'error'>('loading');
  message = signal<string>('');
  readonly appUrl = environment.appUrl;

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      const tenantId = params['tenantId'];

      if (!token || !tenantId) {
        this.status.set('error');
        this.message.set('Link de confirmação inválido. Verifique o e-mail e tente novamente.');
        return;
      }

      this.http.get<{ message: string }>(`${environment.apiUrl}/registration/confirm-email?token=${encodeURIComponent(token)}&tenantId=${encodeURIComponent(tenantId)}`).subscribe({
        next: (res) => {
          this.status.set('success');
          this.message.set(res.message || 'E-mail confirmado com sucesso!');
        },
        error: (err) => {
          this.status.set('error');
          this.message.set(err.error?.message || 'Token de confirmação inválido ou expirado.');
        }
      });
    });
  }
}
