import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import {
  Referral,
  ReferralStatus,
  RewardStatus,
  ReferralStats,
  ReferralProgram,
  ReferralReward,
  ReferralInvitation,
  PaymentMethod
} from '../../models/referral.model';

// Re-export for convenience
export type { Referral, ReferralStats, ReferralProgram, ReferralReward, ReferralInvitation };
export { ReferralStatus, RewardStatus, PaymentMethod };

/**
 * Referral Service
 * 
 * Manages the referral/affiliate program including:
 * - User referral codes and links
 * - Tracking referrals and conversions
 * - Reward management and payouts
 * - Statistics and leaderboards
 * 
 * TODO: Connect to backend API endpoints when available
 */
@Injectable({
  providedIn: 'root'
})
export class ReferralService {
  private readonly REFERRAL_CODE_KEY = 'primecare_referral_code';
  private readonly MOCK_MODE = true;  // Set to false when backend is ready
  
  private programSubject = new BehaviorSubject<ReferralProgram | null>(null);
  public program$: Observable<ReferralProgram | null> = this.programSubject.asObservable();
  
  private statsSubject = new BehaviorSubject<ReferralStats>(this.getMockStats());
  public stats$: Observable<ReferralStats> = this.statsSubject.asObservable();

  constructor() {
    this.initializeProgram();
  }

  /**
   * Initialize the referral program for current user
   */
  private initializeProgram(): void {
    // In production, this would fetch from backend
    // For now, generate a mock program
    const program: ReferralProgram = {
      isActive: true,
      referralCode: this.generateReferralCode(),
      referralLink: this.generateReferralLink(this.generateReferralCode()),
      rewardPerConversion: 100.00,  // R$ 100 per conversion
      minimumPayoutThreshold: 200.00,  // Minimum R$ 200 to request payout
      termsUrl: '/site/referral-terms',
      expirationDays: 90
    };
    
    this.programSubject.next(program);
  }

  /**
   * Generate a unique referral code for a user
   * Format: PRIME-XXXX (e.g., PRIME-A7B9)
   * 
   * NOTE: This is a simple implementation for frontend development.
   * In production, referral codes MUST be generated server-side using
   * cryptographically secure random number generation to prevent
   * code guessing and ensure uniqueness across all users.
   */
  private generateReferralCode(): string {
    const stored = localStorage.getItem(this.REFERRAL_CODE_KEY);
    if (stored) {
      return stored;
    }
    
    // Simple client-side generation for development
    // TODO: Replace with server-side generation in production
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    let code = 'PRIME-';
    for (let i = 0; i < 4; i++) {
      code += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    
    localStorage.setItem(this.REFERRAL_CODE_KEY, code);
    return code;
  }

  /**
   * Generate full referral link with code
   */
  private generateReferralLink(code: string): string {
    const baseUrl = window.location.origin;
    return `${baseUrl}/site/register?ref=${code}`;
  }

  /**
   * Get current user's referral program details
   */
  getReferralProgram(): Observable<ReferralProgram | null> {
    return this.program$;
  }

  /**
   * Get all referrals for current user
   */
  getReferrals(): Observable<Referral[]> {
    if (this.MOCK_MODE) {
      return of(this.getMockReferrals()).pipe(delay(300));
    }
    
    // TODO: Implement backend API call
    // return this.http.get<Referral[]>('/api/referrals');
    return of([]);
  }

  /**
   * Get referral statistics for current user
   */
  getReferralStats(): Observable<ReferralStats> {
    return this.stats$;
  }

  /**
   * Send a referral invitation via email
   */
  sendReferralInvitation(invitation: ReferralInvitation): Observable<{ success: boolean; message: string }> {
    if (this.MOCK_MODE) {
      console.log('Sending referral invitation:', invitation);
      return of({
        success: true,
        message: 'Convite enviado com sucesso!'
      }).pipe(delay(500));
    }
    
    // TODO: Implement backend API call
    // return this.http.post<{success: boolean, message: string}>('/api/referrals/invite', invitation);
    return of({ success: false, message: 'Backend não disponível' });
  }

  /**
   * Get shareable referral link
   */
  getShareableLink(): string {
    const program = this.programSubject.value;
    return program?.referralLink || '';
  }

  /**
   * Share referral link via social media
   */
  shareVia(platform: 'whatsapp' | 'email' | 'linkedin' | 'twitter' | 'copy'): Observable<{ success: boolean; message: string }> {
    const link = this.getShareableLink();
    const message = encodeURIComponent(
      `Conheça o Omni Care Software - o melhor sistema de gestão para clínicas! Use meu código e ganhe desconto: ${link}`
    );
    
    try {
      switch (platform) {
        case 'whatsapp':
          window.open(`https://wa.me/?text=${message}`, '_blank');
          break;
        case 'email':
          window.open(`mailto:?subject=Conheça o Omni Care&body=${message}`, '_blank');
          break;
        case 'linkedin':
          window.open(`https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(link)}`, '_blank');
          break;
        case 'twitter':
          window.open(`https://twitter.com/intent/tweet?text=${message}`, '_blank');
          break;
        case 'copy':
          if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(link).then(
              () => {
                // Return success observable instead of blocking alert
                return of({ success: true, message: 'Link copiado!' });
              },
              () => {
                return of({ success: false, message: 'Erro ao copiar link' });
              }
            );
          } else {
            // Fallback for older browsers
            const textarea = document.createElement('textarea');
            textarea.value = link;
            textarea.style.position = 'fixed';
            textarea.style.opacity = '0';
            document.body.appendChild(textarea);
            textarea.select();
            try {
              document.execCommand('copy');
              document.body.removeChild(textarea);
            } catch (err) {
              document.body.removeChild(textarea);
              return of({ success: false, message: 'Erro ao copiar link' });
            }
          }
          break;
      }
      
      return of({ success: true, message: 'Link compartilhado!' });
    } catch (error) {
      console.error('Error sharing referral link:', error);
      return of({ 
        success: false, 
        message: 'Erro ao compartilhar. Por favor, verifique se pop-ups estão habilitados.' 
      });
    }
  }

  /**
   * Request payout for earned rewards
   */
  requestPayout(amount: number, paymentMethod: PaymentMethod, paymentDetails: string): Observable<{ success: boolean; message: string }> {
    if (this.MOCK_MODE) {
      console.log('Requesting payout:', { amount, paymentMethod, paymentDetails });
      return of({
        success: true,
        message: 'Solicitação de pagamento enviada! Você receberá em até 5 dias úteis.'
      }).pipe(delay(500));
    }
    
    // TODO: Implement backend API call
    // return this.http.post<{success: boolean, message: string}>('/api/referrals/payout', {
    //   amount, paymentMethod, paymentDetails
    // });
    return of({ success: false, message: 'Backend não disponível' });
  }

  /**
   * Track referral from URL parameter
   */
  trackReferralFromUrl(referralCode: string): void {
    // Store referral code in localStorage/sessionStorage for signup
    sessionStorage.setItem('signup_referral_code', referralCode);
    console.log('Tracking referral from code:', referralCode);
    
    // TODO: Send to backend for tracking
    // this.http.post('/api/referrals/track', { referralCode }).subscribe();
  }

  /**
   * Get mock referrals for development
   */
  private getMockReferrals(): Referral[] {
    return [
      {
        id: '1',
        referrerId: 'current-user',
        referrerName: 'Você',
        referredEmail: 'maria@clinica.com',
        referredName: 'Dra. Maria Silva',
        referredUserId: 'user-123',
        status: ReferralStatus.CONVERTED,
        createdAt: new Date('2026-01-15'),
        convertedAt: new Date('2026-01-20'),
        rewardStatus: RewardStatus.PAID,
        rewardValue: 100.00,
        rewardPaidAt: new Date('2026-01-25'),
        referralCode: 'PRIME-A7B9'
      },
      {
        id: '2',
        referrerId: 'current-user',
        referrerName: 'Você',
        referredEmail: 'joao@medico.com',
        referredName: 'Dr. João Santos',
        referredUserId: 'user-124',
        status: ReferralStatus.SIGNED_UP,
        createdAt: new Date('2026-01-22'),
        rewardStatus: RewardStatus.PENDING,
        referralCode: 'PRIME-A7B9'
      },
      {
        id: '3',
        referrerId: 'current-user',
        referrerName: 'Você',
        referredEmail: 'ana@clinica.com',
        status: ReferralStatus.PENDING,
        createdAt: new Date('2026-01-26'),
        rewardStatus: RewardStatus.PENDING,
        referralCode: 'PRIME-A7B9'
      },
      {
        id: '4',
        referrerId: 'current-user',
        referrerName: 'Você',
        referredEmail: 'carlos@hospital.com',
        referredName: 'Dr. Carlos Oliveira',
        referredUserId: 'user-125',
        status: ReferralStatus.CONVERTED,
        createdAt: new Date('2026-01-10'),
        convertedAt: new Date('2026-01-18'),
        rewardStatus: RewardStatus.EARNED,
        rewardValue: 100.00,
        referralCode: 'PRIME-A7B9'
      }
    ];
  }

  /**
   * Get mock statistics for development
   */
  private getMockStats(): ReferralStats {
    return {
      totalReferrals: 4,
      pendingReferrals: 1,
      signedUpReferrals: 1,
      convertedReferrals: 2,
      totalEarned: 200.00,
      totalPaid: 100.00,
      pendingRewards: 100.00,
      conversionRate: 50
    };
  }

  /**
   * Validate referral code format
   */
  isValidReferralCode(code: string): boolean {
    return /^PRIME-[A-Z0-9]{4}$/.test(code);
  }

  /**
   * Get leaderboard (top referrers)
   * TODO: Implement when backend is ready
   */
  getLeaderboard(limit: number = 10): Observable<Array<{ rank: number; name: string; conversions: number; earned: number }>> {
    // Mock data for now
    return of([
      { rank: 1, name: 'Dr. Pedro Alves', conversions: 15, earned: 1500.00 },
      { rank: 2, name: 'Dra. Ana Costa', conversions: 12, earned: 1200.00 },
      { rank: 3, name: 'Dr. Lucas Ferreira', conversions: 10, earned: 1000.00 },
      { rank: 4, name: 'Você', conversions: 2, earned: 200.00 },
      { rank: 5, name: 'Dra. Sofia Martins', conversions: 8, earned: 800.00 }
    ]).pipe(delay(300));
  }
}
