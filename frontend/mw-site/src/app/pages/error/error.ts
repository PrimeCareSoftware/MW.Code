import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';

interface ErrorInfo {
  code: string;
  title: string;
  message: string;
  icon: string;
}

@Component({
  selector: 'app-error',
  imports: [CommonModule, RouterLink],
  templateUrl: './error.html',
  styleUrl: './error.scss'
})
export class ErrorComponent implements OnInit, OnDestroy {
  errorInfo: ErrorInfo = {
    code: '404',
    title: 'Página não encontrada',
    message: 'A página que você está procurando não existe ou foi movida.',
    icon: '🔍'
  };

  private destroy$ = new Subject<void>();

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    // Get error code from route params or query params
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const code = params['code'];
      if (code) {
        this.errorInfo = this.getErrorInfo(code);
      }
    });

    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(queryParams => {
      const code = queryParams['code'];
      if (code) {
        this.errorInfo = this.getErrorInfo(code);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private getErrorInfo(code: string): ErrorInfo {
    const errors: { [key: string]: ErrorInfo } = {
      '400': {
        code: '400',
        title: 'Requisição Inválida',
        message: 'Os dados fornecidos são inválidos. Por favor, verifique e tente novamente.',
        icon: '⚠️'
      },
      '401': {
        code: '401',
        title: 'Não Autorizado',
        message: 'Você precisa estar autenticado para acessar este recurso. Por favor, entre em contato com o suporte se você acredita que isso é um erro.',
        icon: '🔒'
      },
      '403': {
        code: '403',
        title: 'Acesso Negado',
        message: 'Você não tem permissão para acessar este recurso. Por favor, entre em contato com o suporte se você acredita que isso é um erro.',
        icon: '🚫'
      },
      '404': {
        code: '404',
        title: 'Página Não Encontrada',
        message: 'A página que você está procurando não existe ou foi movida.',
        icon: '🔍'
      },
      '408': {
        code: '408',
        title: 'Tempo Esgotado',
        message: 'A operação demorou muito tempo. Por favor, tente novamente.',
        icon: '⏱️'
      },
      '429': {
        code: '429',
        title: 'Muitas Requisições',
        message: 'Você fez muitas requisições. Por favor, aguarde um momento e tente novamente.',
        icon: '⏸️'
      },
      '500': {
        code: '500',
        title: 'Erro no Servidor',
        message: 'Ocorreu um erro interno no servidor. Nossa equipe já foi notificada e está trabalhando para resolver o problema.',
        icon: '🔧'
      },
      '502': {
        code: '502',
        title: 'Gateway Inválido',
        message: 'O servidor está temporariamente indisponível. Por favor, tente novamente em alguns instantes.',
        icon: '🌐'
      },
      '503': {
        code: '503',
        title: 'Serviço Indisponível',
        message: 'O serviço está temporariamente indisponível devido a manutenção ou sobrecarga. Por favor, tente novamente em alguns instantes.',
        icon: '🛠️'
      },
      '504': {
        code: '504',
        title: 'Gateway Timeout',
        message: 'O servidor demorou muito tempo para responder. Por favor, tente novamente.',
        icon: '⏰'
      }
    };

    return errors[code] || errors['404'];
  }

  goBack(): void {
    window.history.back();
  }
}
