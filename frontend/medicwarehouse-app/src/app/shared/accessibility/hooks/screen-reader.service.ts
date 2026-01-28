/**
 * Serviço de Suporte a Leitores de Tela
 * Fornece anúncios dinâmicos para NVDA, JAWS e VoiceOver
 */

import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';

export type AriaLiveMode = 'polite' | 'assertive' | 'off';

@Injectable({
  providedIn: 'root'
})
export class ScreenReaderService {
  private renderer: Renderer2;
  private liveRegionElement: HTMLElement | null = null;

  constructor(rendererFactory: RendererFactory2) {
    this.renderer = rendererFactory.createRenderer(null, null);
    this.initializeLiveRegion();
  }

  /**
   * Inicializa a região de anúncios ao vivo (ARIA live region)
   */
  private initializeLiveRegion(): void {
    // Criar elemento oculto para anúncios
    this.liveRegionElement = this.renderer.createElement('div');
    this.renderer.setAttribute(this.liveRegionElement, 'id', 'sr-live-region');
    this.renderer.setAttribute(this.liveRegionElement, 'role', 'status');
    this.renderer.setAttribute(this.liveRegionElement, 'aria-live', 'polite');
    this.renderer.setAttribute(this.liveRegionElement, 'aria-atomic', 'true');
    this.renderer.setStyle(this.liveRegionElement, 'position', 'absolute');
    this.renderer.setStyle(this.liveRegionElement, 'left', '-10000px');
    this.renderer.setStyle(this.liveRegionElement, 'width', '1px');
    this.renderer.setStyle(this.liveRegionElement, 'height', '1px');
    this.renderer.setStyle(this.liveRegionElement, 'overflow', 'hidden');
    
    this.renderer.appendChild(document.body, this.liveRegionElement);
  }

  /**
   * Anuncia uma mensagem para leitores de tela
   * @param message Mensagem a ser anunciada
   * @param mode Modo de anúncio (polite ou assertive)
   */
  announce(message: string, mode: AriaLiveMode = 'polite'): void {
    if (!this.liveRegionElement) {
      console.warn('Live region não inicializada');
      return;
    }

    // Atualizar o modo aria-live
    this.renderer.setAttribute(this.liveRegionElement, 'aria-live', mode);

    // Limpar conteúdo anterior
    this.liveRegionElement.textContent = '';

    // Aguardar um frame para garantir que o leitor detecte a mudança
    setTimeout(() => {
      if (this.liveRegionElement) {
        this.liveRegionElement.textContent = message;
      }
    }, 100);

    // Limpar após 3 segundos para evitar repetições
    setTimeout(() => {
      if (this.liveRegionElement) {
        this.liveRegionElement.textContent = '';
      }
    }, 3000);
  }

  /**
   * Anuncia uma mensagem de sucesso
   * @param message Mensagem de sucesso
   */
  announceSuccess(message: string): void {
    this.announce(`✓ Sucesso: ${message}`, 'polite');
  }

  /**
   * Anuncia uma mensagem de erro (alta prioridade)
   * @param message Mensagem de erro
   */
  announceError(message: string): void {
    this.announce(`✗ Erro: ${message}`, 'assertive');
  }

  /**
   * Anuncia uma mensagem de aviso
   * @param message Mensagem de aviso
   */
  announceWarning(message: string): void {
    this.announce(`⚠ Aviso: ${message}`, 'polite');
  }

  /**
   * Anuncia uma mensagem informativa
   * @param message Mensagem informativa
   */
  announceInfo(message: string): void {
    this.announce(`ℹ ${message}`, 'polite');
  }

  /**
   * Anuncia navegação entre páginas
   * @param pageName Nome da página
   */
  announceNavigation(pageName: string): void {
    this.announce(`Navegando para ${pageName}`, 'polite');
  }

  /**
   * Anuncia carregamento em progresso
   * @param description Descrição do que está carregando
   */
  announceLoading(description: string): void {
    this.announce(`Carregando ${description}...`, 'polite');
  }

  /**
   * Anuncia conclusão de carregamento
   * @param description Descrição do que foi carregado
   */
  announceLoadComplete(description: string): void {
    this.announce(`${description} carregado`, 'polite');
  }

  /**
   * Limpa a região de anúncios
   */
  clear(): void {
    if (this.liveRegionElement) {
      this.liveRegionElement.textContent = '';
    }
  }

  /**
   * Destroy - limpa recursos
   */
  ngOnDestroy(): void {
    if (this.liveRegionElement) {
      this.renderer.removeChild(document.body, this.liveRegionElement);
      this.liveRegionElement = null;
    }
  }
}
