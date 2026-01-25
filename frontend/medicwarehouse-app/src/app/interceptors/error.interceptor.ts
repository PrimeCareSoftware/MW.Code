import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

/**
 * Interceptor para tratamento de erros HTTP
 * Traduz e sanitiza erros da API para mensagens amigáveis ao usuário
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = getErrorMessage(error);
      
      // Exibe notificação para o usuário apenas se não for erro de autenticação ou autorização
      // (erros de autenticação/autorização são tratados pelas páginas de erro)
      if (error.status !== 401 && error.status !== 403) {
        notificationService.error(errorMessage);
      }

      // Se for erro 401, redireciona apenas para rotas protegidas (não-site)
      if (error.status === 401) {
        const currentUrl = router.url;
        // Se for uma página do site público, não redireciona (permite acesso ao site mesmo com 401)
        if (!currentUrl.startsWith('/site')) {
          // Se for uma página do sistema (protegida), redireciona para página 401
          router.navigate(['/401'], { queryParams: { returnUrl: currentUrl } });
        }
        // Páginas /site permanecem acessíveis mesmo com erros 401
      }

      // Se for erro 403, redireciona para página de acesso negado
      if (error.status === 403) {
        router.navigate(['/403']);
      }

      // Retorna erro com mensagem tratada
      return throwError(() => ({
        ...error,
        userMessage: errorMessage
      }));
    })
  );
};

/**
 * Extrai e traduz mensagem de erro da resposta HTTP
 */
function getErrorMessage(error: HttpErrorResponse): string {
  // Erros de conexão/rede
  if (error.error instanceof ErrorEvent) {
    return 'Erro de conexão. Verifique sua internet e tente novamente.';
  }

  // Erro com mensagem estruturada da API
  if (error.error?.message) {
    return error.error.message;
  }

  // Erro de validação com múltiplos campos
  if (error.error?.errors && typeof error.error.errors === 'object') {
    const validationErrors = Object.values(error.error.errors)
      .flat()
      .filter(msg => typeof msg === 'string')
      .join(', ');
    
    if (validationErrors) {
      return validationErrors;
    }
  }

  // Mensagens padrão baseadas no status HTTP
  switch (error.status) {
    case 400:
      return 'Os dados fornecidos são inválidos. Por favor, verifique e tente novamente.';
    case 401:
      return 'Sua sessão expirou. Por favor, faça login novamente.';
    case 403:
      return 'Você não tem permissão para realizar esta ação.';
    case 404:
      return 'O recurso solicitado não foi encontrado.';
    case 408:
      return 'A operação demorou muito tempo. Por favor, tente novamente.';
    case 409:
      return 'Este registro já existe no sistema.';
    case 422:
      return 'Os dados fornecidos não puderam ser processados.';
    case 429:
      return 'Muitas requisições. Por favor, aguarde um momento e tente novamente.';
    case 500:
      return 'Ocorreu um erro no servidor. Por favor, tente novamente mais tarde.';
    case 502:
    case 503:
    case 504:
      return 'O serviço está temporariamente indisponível. Por favor, tente novamente em alguns instantes.';
    case 0:
      return 'Não foi possível conectar ao servidor. Verifique sua conexão com a internet.';
    default:
      return 'Ocorreu um erro inesperado. Por favor, tente novamente.';
  }
}
